// ***********************************************************************
// Assembly         : ACBr.Net.NFe
// Author           : RFTD
// Created          : 07-27-2014
//
// Last Modified By : RFTD
// Last Modified On : 09-28-2014
// ***********************************************************************
// <copyright file="ProviderBase.cs" company="ACBr.Net">
//		        		   The MIT License (MIT)
//	     		    Copyright (c) 2016 Grupo ACBr.Net
//
//	 Permission is hereby granted, free of charge, to any person obtaining 
// a copy of this software and associated documentation files (the "Software"), 
// to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, 
// and/or sell copies of the Software, and to permit persons to whom the 
// Software is furnished to do so, subject to the following conditions:
//	 The above copyright notice and this permission notice shall be 
// included in all copies or substantial portions of the Software.
//	 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using ACBr.Net.Core.Exceptions;
using ACBr.Net.Core.Extensions;
using ACBr.Net.DFe.Core.Attributes;
using ACBr.Net.DFe.Core.Serializer;
using ACBr.Net.NFSe.Configuracao;
using ACBr.Net.NFSe.Interfaces;
using ACBr.Net.NFSe.Nota;

namespace ACBr.Net.NFSe.Providers
{
    /// <summary>
    /// Class ProviderBase.
    /// </summary>
	public abstract class ProviderBase : IProvider
    {
        #region Constantes
        /// <summary>
        /// The er r_ ms g_ maior
        /// </summary>
        public const string ErrMsgMaior = "Tamanho maior que o m�ximo permitido";
        /// <summary>
        /// The er r_ ms g_ menor
        /// </summary>
        public const string ErrMsgMenor = "Tamanho menor que o m�nimo permitido";
        /// <summary>
        /// The er r_ ms g_ vazio
        /// </summary>
        public const string ErrMsgVazio = "Nenhum valor informado";
        /// <summary>
        /// The er r_ ms g_ invalido
        /// </summary>
        public const string ErrMsgInvalido = "Conte�do inv�lido";
        /// <summary>
        /// The er r_ ms g_ maxim o_ decimais
        /// </summary>
        public const string ErrMsgMaximoDecimais = "Numero m�ximo de casas decimais permitidas";
        /// <summary>
        /// The er r_ ms g_ maio r_ maximo
        /// </summary>
        public const string ErrMsgMaiorMaximo = "N�mero de ocorr�ncias maior que o m�ximo permitido - M�ximo ";
        /// <summary>
        /// The er r_ ms g_ fina l_ meno r_ inicial
        /// </summary>
        public const string ErrMsgFinalMenorInicial = "O numero final n�o pode ser menor que o inicial";
        /// <summary>
        /// The er r_ ms g_ arquiv o_ na o_ encontrado
        /// </summary>
        public const string ErrMsgArquivoNaoEncontrado = "Arquivo n�o encontrado";
        /// <summary>
        /// The er r_ ms g_ soment e_ um
        /// </summary>
        public const string ErrMsgSomenteUm = "Somente um campo deve ser preenchido";
        /// <summary>
        /// The er r_ ms g_ meno r_ minimo
        /// </summary>
        public const string ErrMsgMenorMinimo = "N�mero de ocorr�ncias menor que o m�nimo permitido - M�nimo ";
        /// <summary>
        /// The ds c_ CNPJ
        /// </summary>
        public const string DscCnpj = "CNPJ(MF)";
        /// <summary>
        /// The ds c_ CPF
        /// </summary>
        public const string DscCpf = "CPF";

        #endregion Constantes
        
        #region Fields

        /// <summary>
        /// The xmldoc
        /// </summary>
		protected readonly XmlDocument Xmldoc;
		
		#endregion Fields

		#region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ProviderBase"/> class.
        /// </summary>
		internal ProviderBase(Configuracoes config)
		{
			ListaDeAlertas = new List<string>();
			FormatoAlerta = "TAG:%TAG% ID:%ID%/%TAG%(%DESCRICAO%) - %MSG%.";
			Xmldoc = new XmlDocument();
	        Config = config;}

		#endregion Constructor

		#region Propriedades

        /// <summary>
        /// Gets the lista de alertas.
        /// </summary>
        /// <value>The lista de alertas.</value>
		public List<string> ListaDeAlertas { get; }

        /// <summary>
        /// Gets or sets the formato alerta.
        /// </summary>
        /// <value>The formato alerta.</value>
		public string FormatoAlerta { get; set; }
		
		/// <summary>
		/// Gets or sets a value indicating whether [retirar acentos].
		/// </summary>
		/// <value><c>true</c> if [retirar acentos]; otherwise, <c>false</c>.</value>
		public bool RetirarAcentos { get; set; }

		public Configuracoes Config { get; }

	    public TimeSpan? TimeOut
	    {
		    get
		    {
				TimeSpan? timeOut = null;
				if (Config.WebServices.AjustaAguardaConsultaRet)
					timeOut = TimeSpan.FromSeconds((int)Config.WebServices.AguardarConsultaRet);

			    return timeOut;
		    }
	    }

	    public X509Certificate2 Certificado => Config.Certificados.ObterCertificado();

		#endregion Propriedades

		#region Methods

		#region Public

		public NotaFiscal LoadXml(string xml)
		{
			Guard.Against<FileNotFoundException>(!File.Exists(xml), "Arquivo XML n�o encontrado !");
			var doc = new XmlDocument();
			doc.Load(xml);
			return LoadXml(doc);
		}

		public NotaFiscal LoadXml(Stream stream)
		{
			Guard.Against<ArgumentNullException>(stream == null, "Stream n�o pode ser nulo !");
			var doc = new XmlDocument();
			doc.Load(stream);
			return LoadXml(doc);
		}

		public virtual NotaFiscal LoadXml(XmlDocument xml)
		{
			throw new NotImplementedException("LoadXml");
		}

		public virtual string GetXmlRPS(NotaFiscal nota, bool identado = true, bool showDeclaration = true)
		{
            throw new NotImplementedException("GetXmlRPS");
		}

        public virtual string GetXmlNFSe(NotaFiscal nota, bool identado = true, bool showDeclaration = true)
        {
            throw new NotImplementedException("GetXmlNFSe");
        }

		public virtual RetornoWebService Enviar(int lote, NotaFiscalCollection notas)
	    {
		    throw new NotImplementedException("Fun��o n�o implementada neste Provedor !");
	    }

		#endregion Public

		#region Protected

	    protected string GetUrl(TipoUrl url)
	    {
			switch (Config.WebServices.Ambiente)
		    {
				case TipoAmbiente.Producao:
				    return GetUrlProd(Config.WebServices.CodMunicipio, url);

				default:
					return GetUrlHom(Config.WebServices.CodMunicipio, url);
		    }
	    }
		
		/// <summary>
		/// Adicionars the tag CNPJCPF.
		/// </summary>
		/// <param name="tagCPF">The i d1.</param>
		/// <param name="tagCNPJ">The i d2.</param>
		/// <param name="cnpjcpf">The CNPJCPF.</param>
		/// <returns>XmlElement.</returns>
		protected XmlElement AdicionarTagCNPJCPF(string tagCPF, string tagCNPJ, string cnpjcpf)
		{
			cnpjcpf = cnpjcpf.Trim().OnlyNumbers();

			XmlElement tag = null;
			switch (cnpjcpf.Length)
			{
			    case 11:
			        tag = AdicionarTag(TipoCampo.Str, "CPF", tagCPF, 11, 11, 1, cnpjcpf);
			        if (!cnpjcpf.IsCPF())
			            WAlerta(tagCPF, "CPF", "CPF", ErrMsgInvalido);
			        break;

			    case 14:
			        tag = AdicionarTag(TipoCampo.Str, "CNPJ", tagCNPJ, 14, 14, 1, cnpjcpf);
			        if(!cnpjcpf.IsCNPJ())
			            WAlerta(tagCNPJ, "CNPJ", "CNPJ", ErrMsgInvalido);
			        break;
			}

			if (!cnpjcpf.Length.IsIn(11, 14))
				WAlerta($"{tagCPF}-{tagCNPJ}", "CNPJ-CPF", "CNPJ/CPF", ErrMsgVazio);

			return tag;
		}

        /// <summary>
        /// Adicionars the tag.
        /// </summary>
        /// <param name="tipo">The tipo.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <param name="ocorrencias">The ocorrencias.</param>
        /// <param name="valor">The valor.</param>
        /// <param name="descricao">The descricao.</param>
        /// <returns>XmlElement.</returns>
		protected XmlElement AdicionarTag(TipoCampo tipo, string id, string tag, int min, 
			int max, int ocorrencias, object valor, string descricao = "")
		{
			try
			{
				var conteudoProcessado = string.Empty;
				var estaVazio = valor == null;

				switch (tipo)
				{
					case TipoCampo.Str:
						if (!estaVazio)
							conteudoProcessado = valor.ToString().Trim();
						break;

					case TipoCampo.Dat:
					case TipoCampo.DatCFe:
						if (!estaVazio)
						{
							DateTime data;
							if (DateTime.TryParse(valor.ToString(), out data))
								conteudoProcessado = data.ToString(tipo == TipoCampo.DatCFe ? "yyyyMMdd" : "yyyy-MM-dd");
							else
								estaVazio = true;
						}
						break;

					case TipoCampo.Hor:
					case TipoCampo.HorCFe:
						if (!estaVazio)
						{
							DateTime hora;
							if (DateTime.TryParse(valor.ToString(), out hora))
								conteudoProcessado = hora.ToString(tipo == TipoCampo.HorCFe ? "HHmmss" : "HH:mm:ss");
							else
								estaVazio = true;
						}
						break;

					case TipoCampo.DatHor:
						if (!estaVazio)
						{
							DateTime dthora;
							if (DateTime.TryParse(valor.ToString(), out dthora))
								conteudoProcessado = dthora.ToString("s");
							else
								estaVazio = true;
						}
						break;

					case TipoCampo.DatHorTz:
						if (!estaVazio)
						{
							DateTime dthoratz;
							if (DateTime.TryParse(valor.ToString(), out dthoratz))
								conteudoProcessado = dthoratz.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'sszzz");
							else
								estaVazio = true;
						}
						break;

					case TipoCampo.De2:
					case TipoCampo.De3:
					case TipoCampo.De4:
					case TipoCampo.De6:
					case TipoCampo.De10:
						if (!estaVazio)
						{
							var numberFormat = CultureInfo.InvariantCulture.NumberFormat;
							decimal vDecimal;
							if (decimal.TryParse(valor.ToString(), out vDecimal))
							{
								// ReSharper disable once SwitchStatementMissingSomeCases
								switch (tipo)
								{
									case TipoCampo.De2:
										conteudoProcessado = string.Format(numberFormat, "{0:0.00}", vDecimal);
										break;

									case TipoCampo.De3:
										conteudoProcessado = string.Format(numberFormat, "{0:0.000}", vDecimal);
										break;

									case TipoCampo.De4:
										conteudoProcessado = string.Format(numberFormat, "{0:0.0000}", vDecimal);
										break;

									case TipoCampo.De6:
										conteudoProcessado = string.Format(numberFormat, "{0:0.000000}", vDecimal);
										break;

									default:
										conteudoProcessado = string.Format(numberFormat, "{0:0.0000000000}", vDecimal);
										break;
								}
							}
							else
								estaVazio = true;
						}
						break;

					case TipoCampo.Int:
					case TipoCampo.StrNumberFill:
						if (!estaVazio)
						{
							conteudoProcessado = valor.ToString();
							if (conteudoProcessado.Length < min)
								conteudoProcessado = conteudoProcessado.ZeroFill(min);
						}
						break;

					case TipoCampo.StrNumber:
						if (!estaVazio)
							conteudoProcessado = valor.ToString().OnlyNumbers();
						break;

					case TipoCampo.Enum:
						if (!estaVazio)
						{
							var member = valor.GetType().GetMember(valor.ToString()).FirstOrDefault();
							var enumAttribute = member?.GetCustomAttributes(false).OfType<DFeEnumAttribute>().FirstOrDefault();
							var enumValue = enumAttribute?.Value;
							conteudoProcessado = enumValue ?? valor.ToString();
						}
						break;

					default:
						if (!estaVazio)
							conteudoProcessado = valor.ToString();
						break;
				}

				string alerta;
				if (ocorrencias == 1 && estaVazio && min > 0)
					alerta = ErrMsgVazio;
				else
					alerta = string.Empty;

			    if (!string.IsNullOrEmpty(conteudoProcessado.Trim()) &&
			        (conteudoProcessado.Length < min && string.IsNullOrEmpty(alerta) && conteudoProcessado.Length > 1))
					alerta = ErrMsgMenor;

				if(!string.IsNullOrEmpty(conteudoProcessado.Trim()) && conteudoProcessado.Length > max)
					alerta = ErrMsgMaior;

				if (!string.IsNullOrEmpty(alerta.Trim()) && ErrMsgVazio.Equals(alerta) && !estaVazio)
					alerta += $" [{valor}]";

				WAlerta(id, tag, descricao, alerta);

				XmlElement xmlTag = null;
				if (ocorrencias == 1 && estaVazio)
                    xmlTag = Xmldoc.CreateElement(tag);

				if (estaVazio)
					return xmlTag;

				xmlTag = Xmldoc.CreateElement(tag);
				xmlTag.InnerText = conteudoProcessado;
				return xmlTag;
			}
			catch (Exception ex)
			{
				WAlerta(id, tag, descricao, ex.ToString());
				return Xmldoc.CreateElement(tag);
			}
		}

        /// <summary>
        /// Ws the alerta.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="descricao">The descricao.</param>
        /// <param name="alerta">The alerta.</param>
		protected void WAlerta(string id, string tag, string descricao, string alerta)
		{
			// O Formato da mensagem de erro pode ser alterado pelo usuario alterando-se a property FFormatoAlerta: onde;
			// %TAG%       : Representa a TAG; ex: <nLacre>
			// %ID%        : Representa a ID da TAG; ex X34
			// %MSG%       : Representa a mensagem de alerta
			// %DESCRICAO% : Representa a Descri��o da TAG

			var s = FormatoAlerta.Clone() as string;
			s = s.Replace("%ID%", id).Replace("%TAG%", $"<{tag}>")
				.Replace("%DESCRICAO%", descricao).Replace("%MSG%", alerta);
			
			ListaDeAlertas.Add(s);
        }

		#endregion Protected

		#region Private

		private static string GetUrlHom(int codCidade, TipoUrl url)
		{
			return "";
		}

		private static string GetUrlProd(int codCidade, TipoUrl url)
		{
			return "";
		}

		#endregion Private

		#endregion Methods
	}
}