using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HaloBI.Prism.Plugin
{
    public partial class Template8 : System.Web.UI.Page
    {
        string _contextId = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            _contextId = Request.QueryString["sessionId"];

            if (!Page.IsPostBack)
            {
                var context = GetContext(_contextId);

                // assume iFrame embed mode
                SetContent(context);
            }
        }

        /// <summary>
        /// Initialize plugin on call from client
        /// Set any plugin specific properties here
        /// </summary>
        /// <param name="plugin"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public static object Initialize(object plugin)
        {
            // pull context from session
            var s = JsonConvert.SerializeObject(plugin);
            var w = Newtonsoft.Json.Linq.JObject.FromObject(plugin);
            var sessionId = w["sessionId"].ToString();
            var context = GetContext(sessionId);

            // set plugin properties and store in session
            context["plugin"]["name"] = "Template";
            context["plugin"]["physicalPath"] = Path.GetDirectoryName(
                HttpContext.Current.Request.PhysicalPath);
            context["plugin"]["embedMode"] = "iFrame";
            context["plugin"]["height"] = 800;
            context["plugin"]["width"] = 600;

            // since 15.2.0408 and 15.1 SR1 plugin config information is stored in the Prism View
            // override plugin config by loading from a file if desired. 
            context["plugin"]["config"] = GetConfig(
                     Path.Combine(context["plugin"]["physicalPath"].ToString(),
                     "config.json")
            );

            // Test add timestamp
            //context["plugin"]["config"]["time"] = System.DateTime.Now.ToShortTimeString() + "</></>";

            // save context to session
            SetContextToSession(context, sessionId);

            // return details to client 
            return new
            {
                embedMode = context["plugin"]["embedMode"].ToString()
            };
        }

        /// <summary>
        /// Read config from disk
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static JObject GetConfig(string fileName)
        {
            var text = "";

            using (StreamReader file = File.OpenText(fileName))
            {
                text = file.ReadToEnd();
            }

            return JObject.Parse(text);
        }

        /// <summary>
        /// Set the content
        /// </summary>
        /// <param name="context["plugin"]["config"]></param>
        /// <returns></returns>
        private void SetContent(JObject context)
        {
            // get Header info from config
			uiHeader.Text = context["plugin"]["config"]["headerText"].ToString();
            var members = GetHierarchyMembersFromContext(context);
            uiSelectedMembers.Text = string.Join(",", members.ToArray());

			// set members list dropdown
			SetMembersList(context, uiMembersList);
			if (members.Count > 0)
			{
				var item = uiMembersList.Items.FindByText(members[0].ToString());

				if (item != null)
				{
					item.Selected = true;
				}
		    }

            // now the all important client side hook to update the Prism view
            var viewId = context["view"]["id"].ToString();
            var paneId = context["view"]["paneId"].ToString();
            
            //// this example adds an inline anonymous function to the client side click event 
            //// and prevents the default action of the event (no postback)
            //var clientsideUpdateFunction = string.Format("(function(e) {{ e.preventDefault(); window.parent.halobi.plugin.prismUpdate('{0}','{1}','{2}');}})(event)",
            //        viewId,
            //        paneId,
            //        "" // in iFrame mode context is updated server-side by the plugin
            //);

            // simpler case without preventDefault if you want the asp.net server side event to fire
			//var updateAll = true;
			var clientsideUpdateFunction = string.Format("window.parent.halobi.plugin.prismUpdate('{0}','{1}','{2}', true);",
                    viewId,
                    paneId,
                    ""
			);

            uiUpdatePrism.Attributes.Add("onclick", clientsideUpdateFunction);

            SetDebugInfo(context);
        }

        private void SetMembersList(JObject context, DropDownList ddl)
        {
			var server = context["cube"]["server"].ToString();
			var catalog = context["cube"]["catalog"].ToString();
			var cube = context["cube"]["cube"].ToString();

			// get hierarchy details from config
			var h = context["plugin"]["config"]["hierarchy"];
			var hierarchy = h["name"].ToString();
			var level = h["level"].ToString();
			var allMember = h["allMember"].ToString();
 
			// Build the MDX
			var mdx = "WITH "; 
			mdx += String.Format("MEMBER [Time].[Time].[MemberUniqueName] AS '{0}.CurrentMember.UniqueName' ", 
				hierarchy
			);
			mdx += String.Format("MEMBER [Time].[Time].[MemberName] AS '{0}.CurrentMember.Name' ",
				hierarchy
			);
			mdx += String.Format("SET [Rows] AS '{{Descendants({0}, {1})}}' ",
				allMember,
				level
			);
			mdx += "SELECT{[Time].[Time].[MemberUniqueName], [Time].[Time].[MemberName]} ON COLUMNS, ";
			mdx += "{[Rows]} ON ROWS ";
			mdx += String.Format("FROM [{0}]", 
				cube
			);

			var data = new CubeData(server, catalog, cube);
			var dataSet = data.GetData(mdx);

			// expecting only a single dataTable in the set
			var dataTable = dataSet.Tables[0];

            ddl.Items.Clear();

			foreach (DataRow r in dataTable.Rows)
			{
				ddl.Items.Add(new ListItem(
					r["[Time].[Time].[MemberName]"].ToString(),
					r["[Time].[Time].[MemberUniqueName]"].ToString()
				));
			}
        }

        private void SetDebugInfo(JObject context)
        {
            if (Debug(context))
            {
                uiContext.Visible = true;
                uiContext.Text = context.ToString(Formatting.Indented);
            }
        }

        /// <summary>
        /// Check config debug status
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private bool Debug(JObject context)
        {
            if (context["plugin"]["config"]["debug"] != null &&
                context["plugin"]["config"]["debug"].ToString().ToLower() == "true")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Retrieve the values in the context object associated with the config key
        /// </summary>
        /// <param name="context"></param>
        /// <param name=configKey"></param>
        /// <returns></returns>
        private List<string> GetHierarchyMembersFromContext(JObject context)
        {
            var requriedHierarchy = context["plugin"]["config"]["hierarchy"]["name"].ToString();
            var hierarchies = context["hierarchies"];
            var list = new List<string>();

            foreach (var h in hierarchies)
            {
                if (h["uniqueName"].ToString() == requriedHierarchy)
                {
                    list = JsonConvert.DeserializeObject<List<string>>(h["memberNames"].ToString());
                }
            }

            if (list == null)
            {
                list = new List<string>();
            }

            return list;
        }

        /// <summary>
        /// Load Plugin Context from Session
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        private static JObject GetContext(string sessionId)
        {
            return JObject.Parse(
                HttpContext.Current.Session[sessionId].ToString()
            );
        }

        /// <summary>
        /// Store Plugin Context to Session
        /// </summary>
        /// <param name="context"></param>
        /// <param name="sessionId"></param>
        private static void SetContextToSession(JObject context, string sessionId)
        {
            HttpContext.Current.Session[sessionId] = JsonConvert.SerializeObject(context);
        }

        protected void uiMembersList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var memberUniqueName = uiMembersList.SelectedItem.Value;
            var memberName = uiMembersList.SelectedItem.Text;

            // update the context object
            var context = GetContext(_contextId);
            var requiredHierarchy = context["plugin"]["config"]["hierarchy"]["name"].ToString();
            var hierarchies = context["hierarchies"];
            var list = new List<string>();

            foreach (var h in hierarchies)
            {
                if (h["uniqueName"].ToString() == requiredHierarchy)
                {
                    h["memberUniqueNames"][0] = memberUniqueName;
                    h["memberNames"][0] = memberName;
                }
            }

            SetDebugInfo(context);
            SetContextToSession(context, _contextId);
        }




        protected void uiUpdatePrism_Click(object sender, EventArgs e)
        {
            //
            // Testing Weather API
            //

            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://query.yahooapis.com/v1/public/yql?q=select%20*%20from%20weather.forecast%20where%20woeid%20in%20(select%20woeid%20from%20geo.places(1)%20where%20text%3D%22nome%2C%20ak%22)&format=json&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys");//
            //request.Method = "Get";
            //request.KeepAlive = true;
            //request.ContentType = "application/json";
            ////request.ContentType = "application/x-www-form-urlencoded";

            //HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            ////string myResponse = "";
            ////using (System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream()))
            ////{
            ////    myResponse = sr.ReadToEnd();
            ////}
            //Response.Write(null);

            String returned_data;
            using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
            {
                //client.BaseAddress = new Uri("https://api.stackexchange.com/2.2/");
                HttpResponseMessage response = client.GetAsync("https://query.yahooapis.com/v1/public/yql?q=select%20*%20from%20weather.forecast%20where%20woeid%20in%20(select%20woeid%20from%20geo.places(1)%20where%20text%3D%22nome%2C%20ak%22)&format=json&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys").Result;
                response.EnsureSuccessStatusCode();
                string result = response.Content.ReadAsStringAsync().Result;
                returned_data = result;
            }

            //
            //End Test
            //

            uiSelectedMembers.Text = uiMembersList.SelectedItem.Text + returned_data + "Hello";

        }
    }
}