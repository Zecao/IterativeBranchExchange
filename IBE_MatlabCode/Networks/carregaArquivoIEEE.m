% 
 function alim = carregaArquivoIEEE(sistema)
 
% cria alimentador vazio
alim = criaAlimVazio();
 
switch sistema
     
     case 1 %ok
         alim.Sbase = 100; %ok
         alim.Vbase = 12.66; 
         alim.Fnome = 'alim\Sistema_Zhu2002_3fontes.txt';
         
     case 2 
         alim.Sbase = 10; %ok %orig. 10MVA = 0.194180 % 100MVA = 0.201860379372922
         alim.Vbase = 12.66;
         alim.Fnome = 'alim\Sistema_Zhu2002.txt';
         
     case 3 
         alim.Sbase = 10; %ok % orig. 1000kVA ok.
         alim.Vbase = 12.66;
         alim.Fnome = 'alim\Sistema_Huang2002.txt';
         
     case 31 
         alim.Sbase = 10; %ok % orig. 1000kVA ok.
         alim.Vbase = 12.66;
         alim.Fnome = 'Sistema69.txt';
     
     case 4 
         alim.Sbase = 100; 
         alim.Vbase = 13.8;
         alim.Fnome = 'alim\Sistema_Brasileiro2008.txt';

     case 5 
         alim.Sbase = 100; 
         alim.Vbase = 11.4;
         alim.Fnome = 'alim\Sistema_TPC2003.txt';         
         
     case 6
         alim.Sbase = 100;
         alim.Vbase = 10.0;
         alim.Fnome = 'alim\Sistema_415.txt';
         
    case 7
         alim.Sbase = 100;
         alim.Vbase = 11.0;
         alim.Fnome = 'Sistema119buses';
         
     case 8
         alim.Sbase = 100; % orig. 100000kVA.
         alim.Vbase = 13.8;
         alim.Fnome = '703busesCemigCarrano';
         
     otherwise 
        disp('ERRO');
 end 
 
 end