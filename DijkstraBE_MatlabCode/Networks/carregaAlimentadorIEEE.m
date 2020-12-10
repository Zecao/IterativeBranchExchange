% carrega alimentador IEEE de acordo com o sistema
 function alim = carregaAlimentadorIEEE(sistema)

% Sistemas: 
% 1 Sistema_Zhu2002_3fontes.txt';   % 
% 2 Sistema_Zhu2002.txt';           % 
% 3 Sistema_Huang2002.txt';         %    
% 4 Sistema_Brasileiro2008.txt';    % 
% 5 Sistema_TPC2003.txt';           % 
% 6 Sistema 417 barras 
% 7 119 buses
% 8 703 buses Cemig/Carrano

% carrega arquivo formato IEEE
alim = carregaArquivoIEEE(sistema);

% carrega matriz Bus IEEE
alim = carregaBusIEEE(sistema,alim);

% carrega matriz Branch IEEE
alim = carregaBranchIEEE(sistema,alim);

% carrega Gerador IEEE
alim = carregaGeradorIEEE(sistema,alim);

% carrega Chaves IEEE
alim = carregaChavesIEEE(sistema,alim);

% obtem individuo original 
indOriginal = chaves2individuo(alim.FmChavesIEEE);

% configura Branchs de acordo com individuo original 
% OBS: necessario este codigo pois originalmente o alimentador pode estar
% com todos os branchs fechados (anel).
% OBS2: codigco aqui pq carrego matriz branch primeiro q matriz chaves...
alim = setaBranchsAtivos(alim,indOriginal);

% seta tipo
alim.Ftipo = 'ieee';

% cria FmapVerticesArestas
alim = criaMapVerticesArestas(alim);

 end 
 
 
 