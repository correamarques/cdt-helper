using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;

namespace UnitTestProject1
{
  [TestClass]
  public class LimitesDisponibilidades
  {
    string content, filePath, validContent = string.Empty;
    ConsoleApp1.LimitesDisponibilidades limitesDisponibilidades;

    [TestInitialize]
    public void Setup()
    {
      filePath = @"C:\temp\teste.txt";

      #region Content
      StringBuilder buildContent = new StringBuilder();
      buildContent.AppendLine("64666	 R$10.000,00 ");
      buildContent.AppendLine("17914	 R$10.000,00 ");
      this.content = buildContent.ToString();
      #endregion

      #region ReturnContent
      StringBuilder buildContentReturned = new StringBuilder();
      buildContentReturned.AppendLine("SET NOCOUNT ON");
      buildContentReturned.AppendLine("BEGIN");
      buildContentReturned.AppendLine("\tUPDATE LimitesDisponibilidades SET DisponibGlobalCredito = DisponibGlobalCredito - 10000.00 WHERE ID_CONTA = 64666");
      buildContentReturned.AppendLine("\tUPDATE LimitesDisponibilidades SET DisponibGlobalCredito = DisponibGlobalCredito - 10000.00 WHERE ID_CONTA = 17914");
      buildContentReturned.AppendLine("END");
      this.validContent = buildContentReturned.ToString();
      #endregion

      limitesDisponibilidades = new ConsoleApp1.LimitesDisponibilidades(this.content, this.filePath);
    }

    [TestMethod]
    public void UpdateLimitesDisponibilidades()
    {
      string returnedContent = limitesDisponibilidades.UpdateLimitesDisponibilidades();
      Assert.AreEqual(returnedContent, validContent);
    }

    [TestMethod]
    public void FileNameMustBeTheSame()
    {
      limitesDisponibilidades.UpdateLimitesDisponibilidades();

      FileInfo fileInfoTo = new FileInfo(limitesDisponibilidades.GeneratedFile);
      FileInfo fileInfoFrom = new FileInfo(filePath);

      string fileNameTo = fileInfoTo.Name.Replace(fileInfoTo.Extension, string.Empty);
      string fileNameFrom = fileInfoFrom.Name.Replace(fileInfoFrom.Extension, string.Empty);

      Assert.IsTrue(fileNameFrom.Equals(fileNameTo));
    }

    [TestMethod]
    public void ExtensionMustBeSql()
    {
      limitesDisponibilidades.UpdateLimitesDisponibilidades();

      FileInfo fi = new FileInfo(limitesDisponibilidades.GeneratedFile);
      Assert.IsTrue(fi.Extension.Equals(".sql"));
    }
  }
}
