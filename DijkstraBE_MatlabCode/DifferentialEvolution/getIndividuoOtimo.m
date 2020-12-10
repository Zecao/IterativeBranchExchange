%   retorna individuo otimo
function ind = getIndividuoOtimo(sistema,alim)

global paramAG
oldVAr = paramAG.indTSred;   
paramAG.indTSred =0;

switch sistema
    
    case 1 % 'alim\Sistema_Zhu2002_3fontes.txt';

        ind = [1 1 1 1 1 0 1 1 0 1 0 1 1 1 1 1];
      
    case 2 % alim\Sistema_Zhu2002.txt';

% % Otimo artigo Zhu,2002 0.139532 % s7-s9-s14-s32-s33 
% % OBS: da Inf matpower
%         ind = [1 1 1 1 1 1 0 1 0 1 1 1 1 0 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 ...
% 1 0 0 1 1 1 1 1];  
    
% % otimo (Pegado & Rodriguez, 2018) 137,08 kW,
% % s7-s9-s14-s32-s37 [0.139554253725013]
        ind = [1 1 1 1 1 1 0 1 0 1 1 1 1 0 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 ...
1 1 0 1 1 1 1 0];

    case 3 % 'alim\Sistema_Huang2002.txt';

        ind = [1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 0 1 0 1 1 1 1 1 1 1 1 ...
1 1 0 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 ... 
1 1 1 1 1 1 1 0 1 1 1 0 1 1 1 1 1 ];

% % OBS: nao eh calculado no matpower.
% % Otimo artigo abaixo: 98.59  % 14-56-61-69-70 
% % A.M.Imran, M.Kowsalya and D.P.Kothari, 
% % A novel integration technique for optimal network reconfiguration and distributed generation
% % placement inpower distribution networks
%         
%         % 14-56-61-69-70 
% %         indTS = [24,64,69,18,23]; ou 
%         indTS = [18,23,24,64,69];

    case 4 % 'alim\Sistema_Brasileiro2008.txt';

        % otimo BArbosa et al. 2013
        % 0.280193
        indTS = [7, 35, 51, 90, 96, 106, 118, 126, 135, 137, 138,141,142,...
             144,145,146,147,148,150,151,155]; 
         
        % otimo
        ind = [1 1 1 1 1 1 0 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 0 1 1 1 1 1 1 ...
            1 1 1 1 1 1 1 1 1 0 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 ...
            1 1 1 1 1 0 1 1 1 1 1 0 1 1 1 1 1 1 1 1 1 0 1 1 1 1 1 1 1 1 1 1 1 0 1 1 1 1 1 1 1 0 1 ...
            1 1 1 1 1 1 1 0 1 0 0 1 1 0 0 1 0 0 0 0 0 1 0 0 1 1 1 0 1];

%         % otimo artigo A Modified Branch-Exchange Heuristic Algorithm for Large-Scale Distribution Networks Reconfiguration
%         % troca chave 141 por 152 
%         indTSteste = [7, 35, 51, 90, 96, 106, 118, 126, 135, 137, 138,142,...
%              144,145,146,147,148,150,151,152,155];         
%         % transforma para binario
%         ind = tieSwitch2binario(ind,alim); 

%         % otimo artigo Distribution Network Reconfiguration for Power Loss Reduction and Voltage Profile Improvement Using Chaotic Stochastic Fractal Search Algorithm
%         % aponta perda de 278.9
% %         % nao calcula no Matpower
%             indTSteste =[7, 58, 62, 84, 90, 98, 106, 118, 126, 128, 135, 137, 138, 139, 141, ...
%                 144, 145, 147, 148, 150, 151];
%             
% %         % transforma para binario
%            ind = tieSwitch2binario(indTSteste,alim); 
        
% % otimo retirado do artigo 2012, Imposing Radiality Constraints in Distribution
%  TODO em 2018 constatei erro no individuo abaixo cHAVE 118 NO LUGAR DE
%  ind = [ 1 1 1 1 1 1 0 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 ...
%  1 1 1 1 0 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 0 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 ...
%  1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 0 1 1 1 1 1 0 1 1 1 1 1 1 1 1 ...
%  1 0 1 1 1 1 1 1 1 1 1 1 0 1 1 1 1 1 1 1 1 0 1 1 1 1 1 1 1 1 0 1 0 0 1 1 0 ...
              
    case 5 % 'alim\Sistema_TPC2003.txt';

% Perda calculada: 0.471096540044206

% % ind com chave 41=0 e 42=1 (igual artigo Su &Lee, 2003).
%%ind = [1,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,0,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,0,1,1,0,1,1,0,0,1,0,1,1,1,1];

% % Otimo (chave 42 aberta, no lugar da 41). Perda calculada 469.893131422628
ind = [ 1 1 1 1 1 1 0 1 1 1 1 1 0 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 ...
1 0 1 1 1 1 0 1 1 0 1 1 1 1 1 1 1 1 1 1 1 1 0 1 1 1 1 1 1 0 1 1 1 1 ...
1 1 1 1 1 0 1 1 1 1 1 1 1 1 1 1 0 1 1 0 1 1 0 0 1 0 1 1 1 1 ];

    case 6
        
        % perdas iniciais (Frutuoso, 2017) = 0.7089417
        % perdas iniciais Matpower 0.7087394208972
        % perdas 0.5829837
        
        % (Frutuoso, 2017)  
        % Resultado Matpower = 0.5835
        % 59 chaves ok
        indTS = [1 2 13 15 16 26 31 40 41 50 59 73 82  94 96 97 111 115 136 146 ...
            150 155 156  158 163 168 169 178 179 190 191 194  195 209 230 254 ...
            256 267 270 294 310  321 354 362 385 389 392 395 403 404 423 424 ...
            426 436 437 439 446 449 466];   
        
        % (POSSAGNOLO, 2015) 581.5494
        % resultado Matpower 581.549451945014
        % 59 chaves. ok
        indTS = [5 13 15 16 21 26 31 54 57 59 60 73 86 87 94 96 97 111 115 136 ...
            142 149 150 155 156 158 163 168 169 178 179 191 195 199 209 214 ... 
            254 256 270 294 317 322 325 354 362 369 392 395 403 404 416 423 426 ...
            431 436 437 446 449 466];   
       
        % transforma para binario
        ind = tieSwitch2binario(indTS,alim);       
     
    case 7
        
% #1
%       Zhang 2007 posicao inicial 1294.3
% posicao final % Zhang otimo ITS (valor artigo:  865.86
%         indTS = [43 27 24 52 123 59 40 96 75 72 98 130 131 110 35];

% #2
% H. Ahmadi, J.R. Martí/ (2015) 
% posicao inicial: 1298.1 (Matpower posicao inicial [1296.57295959139 kW])
% otimo: 869.69
% chaves otimo: 
% 23–24, 26–27, 35–36, 41–42, 44–45, 51–65, 53–54, 61–62, 74–75, 77–78, 86–113, 95–100,
% 101–102, 89–110, 114–115
%
%       indTS = [24 27 35 40 43 52 59 72 75 130 96 98 131 110 123]; 

%     Raju and Bijwe % 26 fluxos 870.35
%        indTS= [24,27,35,40,43,52,59,72,75,96,99,110,123,130,131]

%     Mena Garcia e % 53 fluxos 869.726
%        indTS= [24,27,35,40,43,52,59,72,75,96,99,110,123,130,131]

%     Swarnakar  1942 fluxos 
%                24,27,35,40,43,52,59,72,75,96,98,110,123,130,131       

% posicao inicial 1.296572431371664
% % indTS ordenado
       indTS = [24,27,35,40,43,52,59,72,75,96,98,110,123,130,131]; % valor Matlab [869.712853014580]] proximo ao artigo
       
       % transforma para binario
       ind = tieSwitch2binario(indTS,alim); 
       
    case 8
    
        ind = ones(714,1)
            
        
    
end 

paramAG.indTSred =oldVAr;

end

