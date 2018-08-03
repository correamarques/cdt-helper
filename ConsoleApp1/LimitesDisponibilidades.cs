using System;
using System.Globalization;
using System.Text;

namespace ConsoleApp1
{
  public class LimitesDisponibilidades
  {
    public static void Run()
    {
      UpdateLimitesDisponibilidades(@"C:\Temp\ajuste_contas3.txt");
    }


    static void UpdateLimitesDisponibilidades(string filePath)
    {
      System.IO.FileInfo fi = new System.IO.FileInfo(filePath);

      StringBuilder builder = new StringBuilder();

      builder.AppendLine("SET NOCOUNT ON");
      builder.AppendLine("BEGIN");

      foreach (var line in System.IO.File.ReadAllLines(filePath))
      {
        int idConta = Int32.Parse(line.Split('\t')[0]);
        string value = line.Split('\t')[1];

        decimal amount = 0;
        if (value.IndexOf("R$") > 0)
          amount = Decimal.Parse(value.Replace("R$", ""));
        else
          amount = Decimal.Parse(value);

        builder.Append("\tUPDATE LimitesDisponibilidades ");
        builder.AppendFormat("SET DisponibGlobalCredito = DisponibGlobalCredito - {0} ", amount.ToString(new CultureInfo("en-US")));
        builder.AppendFormat("WHERE ID_CONTA = {0}", idConta);
        builder.AppendLine();
      }

      builder.AppendLine("END");
      
      System.IO.File.WriteAllText(fi.FullName.Replace(fi.Extension, ".sql"), builder.ToString(), Encoding.ASCII);
    }
  }
}
