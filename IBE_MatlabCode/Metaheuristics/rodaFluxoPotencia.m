% roda fluxo de potencia
function [ results, perdas ] = rodaFluxoPotencia(sistemaStr,alim) 

% cria struct Matpower
mpc = criaStructMatpower(alim);

global param;

% armazena vetor opcoes do Matpower %help mpoption
% 'OUT_ALL', 0, 'OUT_GEN', 1, 'OUT_BRANCH', 1, 'VERBOSE', 0
opt = mpoption('PF_ALG', 1, 'PF_TOL', 1e-6, 'OUT_ALL', param.OUT_ALL, 'PF_MAX_IT', 50, 'OPF_ALG', 560, 'VERBOSE', 0 ); 

% roda pf e salva resultados em arquivo
casoMatpowerResults = strcat(sistemaStr,'MATPOWER_RESULTS'); 

% seleciona metodo de fluxo
switch param.metodoFluxo
    
    case 1 % PF

        % roda pf
        results = runpf(mpc, opt );
    
    case 11 % PF e results
        
        % roda fluxo de potencia otimo
        results = runpf(mpc, opt, casoMatpowerResults);

    case 2 % PF
        
        % roda fluxo de potencia otimo
        results = runopf(mpc, opt);
        
    case 22 % PF
        
        % roda fluxo de potencia otimo
        results = runopf(mpc, opt, casoMatpowerResults);

    case 3 % metodo de varredura Mateus 

    % 
%     Ieee ieeePlugin = new Ieee();
% 
%     Rede rede = ieee.carregaRede("Sistema_Zhu2002.txt");
% 
%     rede.fluxoVarredura(1);
% 
%     perdas = rede.calculaPerdas();
        
end

% calcula perdas tecnicas nos branchs
perdas = calcPerdas(results);

end

% 
function perdas = calcPerdas(results)

% calcula perdas se houve sucesso
if ( results.success )
    
    ToBusPInj = results.branch(:,14);
%     ToBusQInj = results.branch(:,15);
    FromBusPInj = results.branch(:,16);
%     FromBusQInj = results.branch(:,17);

    P = sum (ToBusPInj + FromBusPInj);
%     Q = sum (ToBusQInj + FromBusQInj);
    
    perdas = P;

else
    
    perdas = inf('single'); 
        
end

end

%  OPF options
%         11 - OPF_ALG, 0             solver to use for AC OPF
%             [    0 - choose default solver based on availability in the     ]
%             [        following order, 540, 560                              ]
%             [  300 - constr, MATLAB Optimization Toolbox 1.x and 2.x        ]
%             [  320 - dense successive LP                                    ]
%             [  340 - sparse successive LP (relaxed)                         ]
%             [  360 - sparse successive LP (full)                            ]
%             [  500 - MINOPF, MINOS-based solver, requires optional          ]
%             [        MEX-based MINOPF package, available from:              ]
%             [        http://www.pserc.cornell.edu/minopf/                   ]
%             [  520 - fmincon, MATLAB Optimization Toolbox >= 2.x            ]
%             [  540 - PDIPM, primal/dual interior point method, requires     ]
%             [        optional MEX-based TSPOPF package, available from:     ]
%             [        http://www.pserc.cornell.edu/tspopf/                   ]
%             [  545 - SC-PDIPM, step-controlled variant of PDIPM, requires   ]
%             [        TSPOPF (see 540)                                       ]
%             [  550 - TRALM, trust region based augmented Langrangian        ]
%             [        method, requires TSPOPF (see 540)                      ]
%             [  560 - MIPS, MATLAB Interior Point Solver                     ]
%             [        primal/dual interior point method (pure MATLAB)        ]
%             [  565 - MIPS-sc, step-controlled variant of MIPS               ]
%             [        primal/dual interior point method (pure MATLAB)        ]
%             [  580 - IPOPT, requires MEX interface to IPOPT solver          ]
%             [        available from: https://projects.coin-or.org/Ipopt/    ]
%             [  600 - KNITRO, requires MATLAB Optimization Toolbox and       ]
%                      KNITRO libraries available from: http://www.ziena.com/ ]
