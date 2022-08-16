% retorna o numero do sistema (rede) de acordo com o nome
function sis = getSistema(nome)

switch nome
    
    case 'alim\Sistema_Zhu2002_3fontes.txt';   
        sis = 1;
        return;
    case 'alim\Sistema_Zhu2002.txt';   
        sis = 2;
        return;
    case 'alim\Sistema_Huang2002.txt';
        sis = 3;
        return;
    case 'alim\Sistema_Brasileiro2008.txt'; 
        sis = 4;
        return;
    case 'alim\Sistema_Brasileiro2008_teste'; 
        sis = 42;
        return;
    case 'alim\Sistema_TPC2003.txt'; 
        sis = 5;
        return;
    case 'alim\Sistema_415.txt'; 
        sis = 6;        
        return;
    case 'Sistema119buses'; 
        sis = 7;        
        return;
        
    otherwise
        sis = [];
end

end
