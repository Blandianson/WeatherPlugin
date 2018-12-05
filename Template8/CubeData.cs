using Microsoft.AnalysisServices.AdomdClient;
using System.Data;
using System.Data.SqlClient;

namespace HaloBI.Prism.Plugin
{
    public class CubeData
	{
		private string Server { get; set; }
		private string Catalog {get; set; }
		private string Cube { get; set; }

		internal CubeData(string server, string catalog, string cube)
		{
			Server = server;
			Catalog = catalog;
			Cube = cube;
		}

		/// <summary>
		/// Return MDX query results as a DataSet
		/// </summary>
		/// <param name="mdx"></param>
		/// <returns></returns>
		internal DataSet GetData(string mdx)
		{
			var builder = new SqlConnectionStringBuilder();
			builder.DataSource = Server;
			builder.InitialCatalog = Catalog;
			DataSet ds = null;

			using (var connection = new AdomdConnection(builder.ToString()))
			{
				connection.Open();
				ds = new DataSet();
				using (var adaptor = new AdomdDataAdapter(mdx, connection))
				{
					adaptor.Fill(ds);
				}
			}
			
			return ds;
		}
	}
}