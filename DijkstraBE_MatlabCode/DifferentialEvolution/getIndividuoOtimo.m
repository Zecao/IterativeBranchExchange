%   retorna individuo otimo
function ind = getIndividuoOtimo(sistema,alim)

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

% % s13-s21-s58-s62-s70, ???
% otimo calculado de Huang, 2002  
% 
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
        
        % OBS: OLD CODE
%         global paramAG
%         oldVAr = paramAG.indTSred;
%         paramAG.indTSred =0;
%        
%         % transforma para binario
%         ind = tieSwitch2binario(indTS,alim); 
%         
%         paramAG.indTSred = oldVAr;

        ind = [1 1 1 1 1 1 0 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 0 1 1 1 1 1 1 ...
            1 1 1 1 1 1 1 1 1 0 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 ...
            1 1 1 1 1 0 1 1 1 1 1 0 1 1 1 1 1 1 1 1 1 0 1 1 1 1 1 1 1 1 1 1 1 0 1 1 1 1 1 1 1 0 1 ...
            1 1 1 1 1 1 1 0 1 0 0 1 1 0 0 1 0 0 0 0 0 1 0 0 1 1 1 0 1];

% % otimo retirado do artigo 2012, Imposing Radiality Constraints in Distribution
%  TODO em 2018 constatei erro no individuo abaixo cHAVE 118 NO LUGAR DE
%  ind = [ 1 1 1 1 1 1 0 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 ...
%  1 1 1 1 0 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 0 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 ...
%  1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 0 1 1 1 1 1 0 1 1 1 1 1 1 1 1 ...
%  1 0 1 1 1 1 1 1 1 1 1 1 0 1 1 1 1 1 1 1 1 0 1 1 1 1 1 1 1 1 0 1 0 0 1 1 0 ...

    case 42 % 'alim\Sistema_Brasileiro2008_teste';

        ind = [1 1 1 1 1 1 0 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 0 1 1 1 1 1 1 ...
            1 1 1 1 1 1 1 1 1 0 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 ...
            1 1 1 1 1 0 1 1 1 1 1 0 1 1 1 1 1 1 1 1 1 0 1 1 1 1 1 1 1 1 1 1 1 0 1 1 1 1 1 1 1 0 1 ...
            1 1 1 1 1 1 1 0 1 0 0 1 1 0 0 1 0 0 0 0 0 1 0 0 1 1 1 0 1];
        
        % 2 ciclos
%         ind = [1	1	1	1	1	1	1	1	1	1	1	1	1	1	1	1	1	1	0	1	1	1	1	1	1	1	1	1	1	1	1	1	1	1	1	1	1	1	1	1	1	1	1	1	1	1	1	1	0	1	1
% ];      
              
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

        % OBS: OLD CODE
        global paramAG
        oldVAr = paramAG.indTSred;
        paramAG.indTSred =0;
       
        % transforma para binario
        ind = tieSwitch2binario(indTS,alim); 
        
        paramAG.indTSred = oldVAr;       
    
end 

end

