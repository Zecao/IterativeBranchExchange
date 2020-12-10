beep off
clc; 
% clear all; 
clear global;
format long;
warning 'off'; 

% reset the random number generator for reproducibility  
rng('default');

% 1 Sistema_Zhu2002_3fontes - 16 buses ;   
% 2 Sistema_Zhu2002 - 33 buses ;           
% 3 Sistema_Huang2002 - 70 buses;  
% 4 Sistema_Brasileiro2008 - 136 buses;  
% 5 Sistema_TPC2003 - 84 buses;
% 6 Sistema 417 barras - 417 buses
% 7 Sistema 119 barras - 119 buses
sistema = 3;

% ALGORITMOS 
% 52 % Differenctial Evolution + Branch Exchange (local search)
% 521 % Only branch Exchange 
algoritmo = 521;

% seta path do aplicativo
setPath();

%numero de execucoes
numexec = 10;

% inicializa var
matrizResultados =[];

for i=1:numexec
    
    % time 1 run
    tstart = tic;
    
    % define variaveis globais
    defineParametrosGerais();
    
    % seta configuracoes do algoritmo genetico
    setaConfiguracoesAG(sistema);
        
    % carregaArquivoAlimentador
    alim = carregaAlimentadorDeTxt(sistema);
     
    % DE Ezequiel 
    [arrayStructElite, populacao, fitness, geracao, migracoes] = algGenDEFluxo( alim, algoritmo, sistema );

    % time 1 run
    telapsed = toc(tstart);
    
    % vetor resultados 
    matrizResultados(:,i) = criaVetorResultados(algoritmo,arrayStructElite,telapsed,migracoes);
        
end

% grava em txt desempenho do AG
plotaInfoAG2(algoritmo, sistema, numexec, matrizResultados);

beep;

% show results
FILENAME = strcat('InfoAG',num2str(algoritmo),'_',num2str(sistema),'_',num2str(numexec),'.txt');
open(FILENAME)