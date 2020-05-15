%retorna paramentros da funcao dado seu nome, como:
%dominio, numero de variaveis, fmin, n adaptado para o calculo de fitness
function  [dominio, nVars, fx, geracoesSugeridas] = rangeFuncao(funcao,alim)

global paramAG

switch funcao
        
    case 'fluxo'

        % nVars eh o numero de branchs p/ alimentadores cemig
        if (strcmp(alim.Ftipo,'cemig'))

            nVars = size(alim.FmBranch,1);    

        else

            % TODO FIX ME 
            nVars = size(alim.FmChavesIEEE(:,5),1);
            
%             if (paramAG.indTSred)
% 
%                 chavesManobraveis = cast(alim.FmChavesIEEE(:,5),'logical');
%                 nVars = size(alim.FmChavesIEEE(chavesManobraveis,5),1);
%             else
%                 nVars = size(alim.FmChavesIEEE,5);
% 
%             end

        end

        dominio = getRange(funcao,nVars);
        fx = 0; 

    case 'CF1'

        nVars = 10; %numero de variaveis 
        dominio = getRange(funcao,nVars);
        fx = 0; %0.11; 
        geracoesSugeridas = 25;

    case 'CF2'

        nVars = 10;            
        dominio = getRange(funcao,nVars);
        fx = 0; %0.12;
         geracoesSugeridas = 25;

    case 'CF3'

        nVars = 10;
        dominio = getRange(funcao,nVars);
        fx = 0; %0.40;
        geracoesSugeridas = 25;%150;    

    case 'CF4'

        nVars = 10;
        dominio = getRange(funcao,nVars);
        fx = 0; % 0.13;
        geracoesSugeridas = 25;

    case 'CF5'

        nVars = 10;
        dominio = getRange(funcao,nVars);
        fx = 0; % 0.22;
        geracoesSugeridas = 25;%120;

    case 'CF6'

        nVars = 10;
        dominio = getRange(funcao,nVars);
        fx = 0; %%-0.22;
        geracoesSugeridas = 25;%80;

    case 'CF7'

        nVars = 10;
        dominio = getRange(funcao,nVars);
        fx = 0; %0.61;
        geracoesSugeridas = 25;%150;

    case 'CF8'

        nVars = 10;
        dominio = getRange(funcao,nVars);
        fx = 0; % -0.48;
        geracoesSugeridas = 25;%80;

    case 'CF9'

        nVars = 10;
        dominio = getRange(funcao,nVars);
        fx = 0; %-0.27;
        geracoesSugeridas = 25;%80;

    case 'CF10'

        nVars = 10;
        dominio = getRange(funcao,nVars);
        fx = 0; % 0.27;
        geracoesSugeridas = 25;% 120;

    case 'rastriginRestrita2'

        nVars = 2; %numero de variaveis 
        dominio = [-5.12, 5.12; -5.12, 5.12]; % Dominio das variaveis da funcao
        fx = 30.2222;
        geracoesSugeridas = 25;%80;

    case 'rastriginRestrita10'

        nVars = 10; %numero de variaveis 
        dominio = 5.12*ones(nVars,2);
        dominio(:,1) = -1*dominio(:,1);
        fx = 151.1111;
        geracoesSugeridas = 25;%300;  

    case 'rastriginRestrita5'

        nVars = 5; %numero de variaveis

        %criacao do dominio
        dominio = 5.12*ones(nVars,2);
        dominio(:,1) = -1*dominio(:,1);

        fx = 0;
        geracoesSugeridas = 25;%300;  
%     n = 1;

    case 'bohachevsky'

        nVars = 5; %numero de variaveis

        %criacao do dominio
        dominio = 5.12*ones(nVars,2);
        dominio(:,1) = -1*dominio(:,1);

        fx = 0;
%             n = 1;

    case 'schwefel'

        nVars = 5; %numero de variaveis 

        %criacao do dominio
        dominio = 30*ones(nVars,2);
        dominio(:,1) = -1*dominio(:,1);

        fx = 1.25;

    case 'rosenbrock'

        nVars = 2; %numero de variaveis

        %criacao do dominio
        dominio = 5*ones(nVars,2);
        dominio(:,1) = -1*dominio(:,1);

        fx = 207.4183; 

    case 'foxHoles'

        nVars = 2;

        %criacao do dominio
        dominio = 65*ones(nVars,2);
        dominio(:,1) = -1*dominio(:,1);

        fx = 0;

    case 'x2'

        nVars = 2;
        dominio = 100*ones(nVars,2);
        dominio(:,1) = -1*dominio(:,1);

    otherwise
        fprintf('\n\nErro. Função não definida');
end
 
end

function range = getRange(name,dim)

    % maximo e minimos p/ cada funcao.
    range = ones(dim,2);

    switch name
        case 'fluxo'
            range(:,1)      =  0;
            range(:,2)      =  1; 
        case 'CF1'
            range(:,1)      =  0; 
        case 'CF2'
            range(1,1)      =  0;
            range(2:dim,1)  = -1;
        case {'CF3','CF4','CF5','CF6','CF7'}
            range(1,1)      =  0;
            range(2:dim,1)  = -2;
            range(2:dim,2)  =  2; 
        case {'CF8'}
            range(1:2,1)    =  0;
            range(3:dim,1)  = -4;
            range(3:dim,2)  =  4;
        case {'CF9','CF10'}
            range(1:2,1)    =  0;
            range(3:dim,1)  = -2;
            range(3:dim,2)  =  2;                   
    end

end
