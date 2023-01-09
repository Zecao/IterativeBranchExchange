using ExecutorOpenDSS.Classes_Auxiliares;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;
using System.IO;
using System.Xml.Linq;

namespace ExecutorOpenDSS
{
    class XMLConfigurationFile
    {
        //Lê o arquivo XML com as configurações
        public static bool GetConfiguracoes(MainWindow janela, string arquivo)
        {
            //Verifica se o arquivo existe
            if(File.Exists(arquivo)) 
            {
                try
                {
                    //Lê o XML
                    XDocument xDoc = XDocument.Load(arquivo);
                    
                    //Pega o elemento root
                    XElement raiz = xDoc.Root;
                   
                    //Define o caminho
                    janela.caminhoDSSTextBox.Text = raiz.Element("CaminhoMensais").Value;

                    //Define o caminho recursos permanentes
                    janela.caminhoPermTextBox.Text = raiz.Element("CaminhoRecPerm").Value;                    
                   
                    //Define o tipo de fluxo
                    janela.tipoFluxoComboBox.Text = raiz.Element("TipoFluxo").Value;
                   
                    //Define o tipo de dia
                    janela.tipoDiaComboBox.Text = raiz.Element("TipoDia").Value;

                    //Define o mês
                    janela.mesComboBox.Text = raiz.Element("Mes").Value;

                    //Define o ano
                    janela.anoTextBox.Text = raiz.Element("Ano").Value;

                    //Define checkbox Otimiza LoadMult
                    janela.otimizaEnergiaCheckBox.IsChecked = Convert.ToBoolean(raiz.Element("OtmEnergiaChecked").Value);

                    //Define checkbox simplificaMesComDU
                    janela.simplificaMesComDUCheckBox.IsChecked = Convert.ToBoolean(raiz.Element("SimplificaMesComDUCheckBox").Value);

                    //Define checkbox Otimiza Energia 
                    janela.otimizaCheckBox.IsChecked = Convert.ToBoolean( raiz.Element("OtmChecked").Value );

                    //Define o incremento do ajuste
                    janela.incrementoAjusteTextBox.Text = raiz.Element("IncrementoAjuste").Value;                                   
                                        
                    //Define o check-box usar tensao barramento
                    janela.usarTensoesBarramentoCheckBox.IsChecked = Convert.ToBoolean(raiz.Element("UsarTensaoBar").Value);

                    //Define o TextBox tensaoPU default
                    janela.tensaoSaidaBarTextBox.Text = raiz.Element("TensaoPUDefault").Value;

                    //Adiciona o loadMult alternativo
                    janela.loadMultAltTextBox.Text = raiz.Element("loadMultAlternativo").Value;

                    // AllowFormsCheckBox
                    janela.AllowFormsCheckBox.IsChecked = Convert.ToBoolean(raiz.Element("AllowForms").Value);

                    // Hora usuario
                    janela.horaTextBox.Text = raiz.Element("Hora").Value;

                    // Populates Expander parameters
                   janela._parGUI._expanderPar = new ExpanderParameters(raiz,janela);

                    return true;
                }
                catch
                {
                    // TODO
                    return false;
                }
                
            }
            else
            {
                // Retorna falso caso o arquivo não exista
                return false;
            }
        }

        //Grava o arquivo XML com as configurações
        public static void SetConfiguracoes(MainWindow janela, string arquivo)
        {
            //Cria um novo arquivo xml
            XDocument xDoc = new XDocument(new XDeclaration("1.0", "utf-8", "true"));
            
            //Cria o elemento root
            XElement raiz = new XElement("Configuracoes");
            
            //Adiciona o caminho
            raiz.Add(new XElement("CaminhoMensais", janela.caminhoDSSTextBox.Text));

            //Adiciona o caminho
            raiz.Add(new XElement("CaminhoRecPerm", janela.caminhoPermTextBox.Text));

            //Adiciona o mês
            raiz.Add(new XElement("Mes", janela.mesComboBox.Text));

            //Adiciona o mês
            raiz.Add(new XElement("Ano", janela.anoTextBox.Text));
            
            //Adiciona se deve permitir mensagens de erro do OpenDSS
            raiz.Add(new XElement("AllowForms", janela.AllowFormsCheckBox.IsChecked.Value));
            
            //Adiciona o tipo de fluxo
            raiz.Add(new XElement("TipoFluxo", janela.tipoFluxoComboBox.Text));
            
            //Adiciona o tipo de dia
            raiz.Add(new XElement("TipoDia", janela.tipoDiaComboBox.Text));

            //Adiciona se a otimização está habilitada
            raiz.Add(new XElement("OtmChecked", janela.otimizaCheckBox.IsChecked.Value));

            //Adiciona se a otimização energia está habilitada
            raiz.Add(new XElement("OtmEnergiaChecked", janela.otimizaEnergiaCheckBox.IsChecked.Value));

            //Opcao Simplificacao mes por DU  
            raiz.Add(new XElement("SimplificaMesComDUCheckBox", janela.simplificaMesComDUCheckBox.IsChecked.Value));

            //Adiciona o incremento do ajuste
            raiz.Add(new XElement("IncrementoAjuste", janela.incrementoAjusteTextBox.Text));

            //Adiciona o usarTensoesBarramentoCheckBox
            raiz.Add(new XElement("UsarTensaoBar", janela.usarTensoesBarramentoCheckBox.IsChecked.Value));

            //Adiciona a tensaoPU default
            raiz.Add(new XElement("TensaoPUDefault", janela.tensaoSaidaBarTextBox.Text));

            //Adiciona o loadMult alternativo
            raiz.Add(new XElement("loadMultAlternativo", janela.loadMultAltTextBox.Text));

            // Adiciona Hora usuario
            raiz.Add(new XElement("Hora", janela.horaTextBox.Text));
                        
            //Adiciona calculaDRPDRCCheckBox
            raiz.Add(new XElement("CalculaDRPDRC", janela._parGUI._expanderPar._calcDRPDRC));
            
            //Adiciona _otimizaPUSaidaSE
            raiz.Add(new XElement("CalculaPUOtm", janela._parGUI._expanderPar._otimizaPUSaidaSE ));
                        
            //Adiciona _calcTensaoBarTrafo
            raiz.Add(new XElement("CalcTensaoBarTrafo", janela._parGUI._expanderPar._calcTensaoBarTrafo));

            //Adiciona _verifCargaIsolada
            raiz.Add(new XElement("VerifCargaIsolada", janela._parGUI._expanderPar._verifCargaIsolada));

            //Adiciona IncluirCapMT
            raiz.Add(new XElement("IncluirCapMT", janela._parGUI._expanderPar._incluirCapMT));

            //Adiciona ModeloCargaCemig
            raiz.Add(new XElement("ModeloCargaCemig", janela._parGUI._expanderPar._modeloCargaCemig));

            //Adiciona RelatorioTapsRTs
            raiz.Add(new XElement("RelatorioTapsRTs", janela._parGUI._expanderPar._verifTapsRTs));

            //Adiciona o StringBatchEdit 
            raiz.Add(new XElement("StringBatchEdit", janela._parGUI._expanderPar._strBatchEdit));

            //Adiciona o root ao XML
            xDoc.Add(raiz);

            //SAlva o arquivo XML
            xDoc.Save(arquivo);
        }
    }
}
