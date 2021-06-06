# DijkstraBranchExchange
Repository for the article "Reconfiguration of Real Distribution Networks using Branch Exchange and Differential Evolution Algorithm to Reduce Power Losses".
This repository is subdivided in the following subdirectories: 

## 1. Article Matlab code
[**"DijkstraBEMatlabCode"**](https://github.com/Zecao/2020Dijkstra/tree/master/DijkstraBE_MatlabCode)
Directory with the Matlab code for replication of all the results from article.

Things to note:
1. The main scripts files are named "run_XXbuses.m" which XX is the system buses number. 
2. The codes uses the [Matpower](https://matpower.org/) Power Flow routines and the [Matlab-BGL](https://github.com/dgleich/matlab-bgl) graph library. So remember to edit the function "setPath.m" properly.
3. The code is mainly with portuguese comments, so I apologize for the non-portugues speaking.  

## 2. Article OpenDSS customization code
[**"DijkstraBE_OpenDSS"**](https://github.com/Zecao/2020Dijkstra/tree/master/DijkstraBE_OpenDSS)
Directory with the Visual Studio C# OpenDSS customization project. Essencially its a newer version from the project https://github.com/Zecao/ExecutorOpenDssBr
The project uses the graph data structures and algorithms from [QuickGraph 3.6](https://archive.codeplex.com/?p=quickgraph)
The project also uses 2 dll files: EEPlus.dll to read Excel files and Auxiliares.dll already included in the project. 

## 3. Cemig-D Distribution feeders
[**"CemigFeeders"**](https://github.com/Zecao/2020Dijkstra/tree/master/CemigDFeeders)
Directory with the OpenDSS files (.dss) from 3 Cemig distribution feeders and 2 substations. These files are public as they were generated from a geographic distribution data base BDGD from Brazilian Regulatory Agency ANEEL.

## 4. Initial population of evolutionary metaheuristics for the Distribution Network Reconfiguration problem
[**"MSTInitialPopulation"**](https://github.com/Zecao/2020Dijkstra/tree/master/MSTInitialPopulation)
Directory with the initial populations used in the article for the Distribution Network Reconfiguration - DNR problem. These files are in Matlab format (.mat) and uses the binary codification, i.e. they represent the normally closed (NC) switches by 1s and the normally opened (NO) switches by 0s for the following test cases:  
* (Zhu, 2002) 33 buses system
* (Huang, 2002) 69 (or 70) buses system
* (Su & Lee, 2003) 84 buses system
* (Guimarães & Castro, 2005) 136 buses system
* (Ramirez-rosado & Bernal-Agustín, 1998) 417 (or 415) buses system
* (Zhang, 2007) 119 buses system.

Also, the OpenDSS files (.dss) for these networks and some references works can be found here: (https://www.dropbox.com/sh/15fo64kxz7115e2/AAAGgAVzJmnLzA6rVlqfBdGca?dl=0)

Ezequiel C. Pereira
