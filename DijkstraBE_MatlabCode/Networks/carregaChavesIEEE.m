function alim = carregaChavesIEEE(sistema,alim)

% 1 Sistema_Zhu2002_3fontes.txt';    
% 2 Sistema_Zhu2002.txt';           
% 3 Sistema_Huang2002.txt';           
% 4 Sistema_Brasileiro2008.txt';   
% 5 Sistema_TPC2003.txt';  
% 6 Sistema 417

switch sistema
    
    case 1 %
        alim.FmChavesIEEE = CHAVESSistemaZhu20023fontes();
         
    case 2 %
        alim.FmChavesIEEE = CHAVESSistemaZhu2002();
        
    case 3 %
        alim.FmChavesIEEE = CHAVESSistemaHuang2002();
    
    case 4 %

        alim.FmChavesIEEE = CHAVESSistema136();
        
    case 42 %

        alim.FmChavesIEEE = CHAVESSistema136_teste();
           
    case 5 %        
        alim.FmChavesIEEE = CHAVESSistemaTPC2003();
       
    case 6
        alim.FmChavesIEEE = CHAVESSistema417();
        
end 

end
