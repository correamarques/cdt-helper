using System;
using System.Globalization;
using System.Text;

namespace ConsoleApp1
{
  public class LimitesDisponibilidades
  {
    readonly string content, filePath = string.Empty;

    public string Content => content;
    public string GeneratedFile { get; set; }


    public LimitesDisponibilidades(string content, string filePath)
    {
      this.content = content;
      this.filePath = filePath;
    }


    public static void Run()
    {
      string filePath = @"C:\Temp\ajuste_contas3.txt";
      string content = System.IO.File.ReadAllText(filePath);

      LimitesDisponibilidades limitesDisponibilidades = new LimitesDisponibilidades(content, filePath);
      limitesDisponibilidades.UpdateLimitesDisponibilidades();
    }


    public string UpdateLimitesDisponibilidades()
    {
      System.IO.FileInfo fi = new System.IO.FileInfo(filePath);

      StringBuilder builder = new StringBuilder();

      builder.AppendLine("SET NOCOUNT ON");
      builder.AppendLine("BEGIN");

      string[] lines = content.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

      foreach (var line in lines)
      {
        if (line.Length <= 5)
          break;

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

      GeneratedFile = fi.FullName.Replace(fi.Extension, ".sql");
      System.IO.File.WriteAllText(GeneratedFile, builder.ToString(), Encoding.ASCII);

      return builder.ToString();
    }
  }
}
