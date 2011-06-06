using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using System.Net;
using System.IO;
using System.Text;
using System.Xml;
using System.Dynamic;

namespace ConsultaCEP.Controllers
{
    public class CepController : Controller
    {
        //
        // GET: /Cep/

        public JsonResult Consulta(string cep)
        {
            var sb = new StringBuilder();

            var buf = new byte[8192];

            Uri uri = new Uri("http://www.buscacep.correios.com.br/servicos/dnec/consultaLogradouroAction.do?CEP=" + cep + "&Metodo=listaLogradouro&TipoConsulta=cep&StartRow=1&EndRow=10");

            HttpWebRequest request = (HttpWebRequest)
             WebRequest.CreateDefault(uri);

            HttpWebResponse response = (HttpWebResponse)
             request.GetResponse();

            Stream resStream = response.GetResponseStream();

            string tempString = null;
            int count = 0;

            do
            {
                count = resStream.Read(buf, 0, buf.Length);

                if (count != 0)
                {
                    tempString = Encoding.UTF8.GetString(buf, 0, count);
                    sb.Append(tempString);
                }
            }
            while (count > 0);

            sb.Insert(0, "<root>");
            sb.Remove(6, 12380);
            sb.Remove(311, sb.Length - 311);
            sb.Append("</root>");


            var xml = XElement.Parse(sb.ToString());

            dynamic endereco = new ExpandoObject();

            if (xml.Elements().ToList().Count == 0)
            {
                endereco.Mensagem = "Cep não encontrado!";
                return Json(endereco, JsonRequestBehavior.AllowGet);
            }

            endereco.Rua = xml.Elements().ToList()[0].Value;
            endereco.Bairro = xml.Elements().ToList()[1].Value;
            endereco.Cidade = xml.Elements().ToList()[2].Value;
            endereco.Estado = xml.Elements().ToList()[3].Value;
            endereco.Mensagem = "Sucesso";

            return Json(endereco, JsonRequestBehavior.AllowGet);
        }

    }
}