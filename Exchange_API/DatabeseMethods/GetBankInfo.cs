
using Microsoft.Data.SqlClient;
using System.ComponentModel;
using Exchange_API.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Exchange_API.Data
{
	public class GetBankInfo : ExchangeRate
	{
		private static readonly string connectionString =
			System.Configuration.ConfigurationManager.ConnectionStrings["DefaultDb"].ConnectionString;
		public static Dictionary<string, ExchangeRate> NonParameterGetter()
		{
			Dictionary<string, ExchangeRate> exchangeRates = new Dictionary<string, ExchangeRate>();
			try
			{
				using (SqlConnection connection = new SqlConnection(connectionString))
				{
					connection.Open();
					using (SqlCommand command = new SqlCommand(
							   "SELECT DISTINCT bank.codebank, bank.namebank, currency.namecurrency FROM exchange_rate JOIN bank ON bank.id = exchange_rate.bank_id JOIN currency ON currency.id = exchange_rate.currency_id;",
							   connection))
					{
						using (SqlDataReader reader = command.ExecuteReader())
						{
							while (reader.Read())
							{
								string bankName = reader["namebank"].ToString();
								if (exchangeRates.ContainsKey(bankName))
								{
									exchangeRates[bankName].CurrencyName += ", " + reader["namecurrency"].ToString();
								}
								else
								{
									ExchangeRate rate = new ExchangeRate()
									{
										BankCode = reader["codebank"].ToString(),
										BankName = bankName,
										CurrencyName = reader["namecurrency"].ToString()
									};
									exchangeRates.Add(bankName, rate);
								}
							}
						}
					}

					connection.Close();
					return exchangeRates;
				}
            }
			catch(SqlException ex)
			{
				return exchangeRates = null;

            }

        }

		public static List<ExchangeRate> ParameterGetter(List<object> objects)
		{
			var exChange = new ExchangeRate();
			List<ExchangeRate> exchangeRates = new List<ExchangeRate>();
			string query = "SELECT bank.codebank, bank.namebank, currency.codecurrency, currency.namecurrency, exchange_rate.buy_rate, exchange_rate.sell_rate, exchange_rate.date FROM exchange_rate INNER JOIN bank ON exchange_rate.bank_id = bank.id INNER JOIN currency ON exchange_rate.currency_id = currency.id WHERE currency.codecurrency = @currency AND bank.namebank = @bankName";

			if (objects[2] != null && objects[3] != null)
			{
				query += " AND exchange_rate.date BETWEEN @startDate AND @endDate";
			}
			if (objects[2] == null && objects[3] == null)
			{
				query += " ORDER BY ABS(exchange_rate.buy_rate - exchange_rate.sell_rate) DESC";
			}

			if (objects[2] != null && objects[3] == null)
			{
				query += " AND exchange_rate.date = @startDate";
			}
			if (objects[2] == null && objects[3] != null)
			{
				query += " AND exchange_rate.date = @endDate";
			}
			try
			{
				using (SqlConnection connection = new SqlConnection(connectionString))
				{
					connection.Open();

					foreach (string item in (List<string>)objects[1])
					{
						SqlCommand command = new SqlCommand(query, connection);
						command.Parameters.AddWithValue("@currency", objects[0].ToString());
						command.Parameters.AddWithValue("@bankName", item);
							if (objects[2] != null && objects[3] != null)
							{
								command.Parameters.AddWithValue("@startDate", Convert.ToDateTime(objects[2]));
								command.Parameters.AddWithValue("@endDate", Convert.ToDateTime(objects[3]));
							}
							if (objects[2] != null && objects[3] == null)
							{
								command.Parameters.AddWithValue("@startDate", Convert.ToDateTime(objects[2]));
							}
							if (objects[2] == null && objects[3] != null)
							{
								command.Parameters.AddWithValue("@endDate", Convert.ToDateTime(objects[3]));
							}
						SqlDataReader reader = command.ExecuteReader();
						
						while (reader.Read())
						{
							exChange = new ExchangeRate
							{
								BankCode = reader["codebank"].ToString(),
								CurrencyName = reader["codecurrency"].ToString(),
								BayRate = (decimal)reader["buy_rate"],
								SellRate = (decimal)reader["sell_rate"],
								RateDate = (DateTime)reader["date"]
							};
							exchangeRates.Add(exChange);
						}
						reader.Close();
					}
					exchangeRates = exchangeRates.OrderBy(x => Math.Abs(x.BayRate - x.SellRate)).ToList();
					return exchangeRates;
				}
			}
			catch (SqlException e)
			{

				exChange = new ExchangeRate
				{
					ErrorFlag = "Server error"
				};
				exchangeRates.Add(exChange);

				return exchangeRates;
			}
		}


		public static int AddCurrenc(string CurCode,string Fulname)
		{
			var exChange = new ExchangeRate();

			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();

				try
				{

					using (SqlCommand command = new SqlCommand())
					{
						command.Connection = connection;
						command.CommandText =
							"INSERT INTO currency (codecurrency, namecurrency) VALUES (@codecurrency, @namecurrency)";
						command.Parameters.AddWithValue("@codecurrency", CurCode);
						command.Parameters.AddWithValue("@namecurrency", Fulname);

						command.ExecuteNonQuery();
						exChange.ErrorFlag = "Complited";
					}
				}
				catch (SqlException e)
				{
					return 0;
				}
				finally
				{
					connection.Close();
				}
				return 1;
			}
		}


		public static int ChackerCurency(string CurCode)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();
				using (SqlCommand command = new SqlCommand($"SELECT * FROM currency WHERE codecurrency = '{CurCode}'",
					       connection))
				{
					using (SqlDataReader reader = command.ExecuteReader())
					{
						if (reader.HasRows)
						{
							return 99;
						}
						else
						{
							return 100;
						}
					}
				}
			}

		}

		public static int Belonging_to_the_bank(List<object> openup)
		{
			var exchange = new ExchangeRate();

			string insertQuery = "INSERT INTO exchange_rate (bank_id, currency_id, buy_rate, sell_rate, date) " +
			                     "SELECT bank.id, currency.id, @buyRate, @sellRate, @date " +
			                     "FROM bank INNER JOIN currency ON bank.codebank = @CodeBank AND currency.codecurrency = @currency";

			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();

				try
				{
					using (SqlCommand command = new SqlCommand())
					{
						command.Connection = connection;
						command.CommandText = insertQuery;
						command.Parameters.AddWithValue("@currency", openup[0].ToString());
						command.Parameters.AddWithValue("@CodeBank", openup[1].ToString());
						command.Parameters.AddWithValue("@buyRate", openup[2]);
						command.Parameters.AddWithValue("@sellRate", openup[3]);
						command.Parameters.AddWithValue("@date", DateTime.Now.Date);
						command.ExecuteNonQuery();

					}
				}
				catch (SqlException ex)
				{
					if (ex.Number == 11001)
					{
						return 0;
					}
				}
				finally
				{
					connection.Close();
				}
				return 1;
			}

			
		}

	}
}