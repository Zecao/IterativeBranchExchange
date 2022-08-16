function alim = carregaGeradorIEEE(opcao,alim)

switch opcao
    
    case 1 % 'alim\Sistema_Zhu2002_3fontes.txt';
                
        A = SistemaZhu20023fontesA();
        
        numGer = 3;
        A = A(1:numGer,:);
        
        %cria matriz mGerador
        mGerador = zeros(size(A,1),21);
        mGerador = setaPotenciaGerador(mGerador,16,4); %11,3
               
    case 2 % 'alim\Sistema_Zhu2002.txt';
        
        A = SistemaZhu2002A();
        A = A(1,:);
        
        %cria matriz mGerador
        mGerador = zeros(size(A,1),21);
        mGerador = setaPotenciaGerador(mGerador,4.5,3);
           
    case 3 % 'alim\Sistema_Huang2002.txt';
        
        A = SistemaHuang2002A();
        A = A(1,:);
        
        %cria matriz mGerador
        mGerador = zeros(size(A,1),21);
        mGerador = setaPotenciaGerador(mGerador,4.3,2.9); % 4.2,2.8
            
    case 4 % 'alim\Sistema_Brasileiro2008.txt';
        
        A = Sistema136barras();
        
        A = A(1,:);
        
        %cria matriz mGerador
        mGerador = zeros(size(A,1),21);
        mGerador = setaPotenciaGerador(mGerador,20,10);
                 
    case 5 % 'alim\Sistema_TPC2003.txt
        
        A = SistemaTPC2003A();
        A = A(1:11,:);
        
        %cria matriz mGerador
        mGerador = zeros(size(A,1),21);
        mGerador = setaPotenciaGerador(mGerador,4.0,3.0); 
        
    case 6
        
        A = Sistema417barras();
        
        numGer = 1;
        A = A(1:numGer,:);    
        
        %cria matriz mGerador       
        mGerador = zeros(size(A,1),21);
        mGerador = setaPotenciaGerador(mGerador,4.0,3.0); 
        
    case 7
        
        A = Sistema119barras();        
        
        % indices doss geradores 
        indGerador = find(A(:, 2)==3);
        
        A = A(indGerador,:);    
        
        %cria matriz mGerador       
        mGerador = zeros(size(A,1),21);
        mGerador = setaPotenciaGerador(mGerador,4.0,3.0);
        
    case 8
        
        A = Sistema703barrasCemig();        
        
        % indices doss geradores 
        indGerador = find(A(:, 2)==3);
        
        A = A(indGerador,:);    
        
        %cria matriz mGerador       
        mGerador = zeros(size(A,1),21);
        mGerador = setaPotenciaGerador(mGerador,40.0,30.0);
        
    otherwise 
        disp('ERRO');
        
end 

% cria matrizes geradores
% bus	Pg	Qg	Qmax	Qmin	Vg	mBase	status	Pmax	Pmin	Pc1	Pc2	Qc1min	Qc1max	Qc2min	Qc2max	ramp_agc	ramp_10	ramp_30	ramp_q	apf
mGerador(:,1) = A(:,1); % bus number

mGerador(:,6) = 1.0; % voltage magnitude setpoint (p.u.)
mGerador(:,7) = alim.Sbase; % total MVA base of machine, defaults to baseMVA 
mGerador(:,8) = 1; % machine status

mGerador(:,11) = 0; % lower real power output of PQ capability curve (MW)
mGerador(:,12) = 0; % upper real power output of PQ capability curve (MW)
mGerador(:,13) = 0; % minimum reactive power output at PC1 (MVAr)
mGerador(:,14) = 0; % maximum reactive power output at PC1 (MVAr)
mGerador(:,15) = 0; % minimum reactive power output at PC2 (MVAr)
mGerador(:,16) = 0; % maximum reactive power output at PC2 (MVAr)
mGerador(:,17) = 0; % ramp rate for load following/AGC (MW/min)
mGerador(:,18) = 0; % ramp rate for 10 minute reserves (MW)
mGerador(:,19) = 0; % ramp rate for 30 minute reserves (MW)
mGerador(:,20) = 0; % ramp rate for reactive power (2 sec timescale) (MVAr/min)
mGerador(:,21) = 0; % area participation factor

%
alim.FmGerador = mGerador;

% setNoRaiz
alim.FbarraIdCab = mGerador(1,1);

end

% 
function mGerador = setaPotenciaGerador(mGerador,real,reativo) 

mGerador(:,2) = real; % real power output (MW)

mGerador(:,3) = 0; % reactive power output (MVAr)

mGerador(:,4) = reativo; % maximum reactive power output (MVAr)
mGerador(:,5) = -reativo; % minimum reactive power output (MVAr)

mGerador(:,9) = real*1.0; % maximum real power output (MW)
mGerador(:,10) = real*0.25; % minimum real power output (MW)

end