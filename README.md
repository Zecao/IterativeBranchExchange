# Iterative Branch Exchange
Repository for the article "Distribution Networks Reconfiguration using an iterative Branch Exchange and metaheuristics".
This repository is subdivided in the following subdirectories: 

## 1. Initial population of evolutionary metaheuristics for the Distribution Network Reconfiguration problem
[**"MSTInitialPopulation"**](https://github.com/Zecao/2020Dijkstra/tree/master/MSTInitialPopulation)
Directory with the initial populations used in the article for the Distribution Network Reconfiguration - DNR problem. These files are in Matlab format (.mat) and uses the binary codification, i.e. they represent the normally closed (NC) switches by 1s and the normally opened (NO) switches by 0s for the following test cases:  
* (Zhu, 2002) 33 buses system
* (Huang, 2002) 69 (or 70) buses system
* (Su & Lee, 2003) 84 buses system
* (Guimarães & Castro, 2005) 136 buses system
* (Ramirez-rosado & Bernal-Agustín, 1998) 417 (or 415) buses system
* (Zhang, 2007) 119 buses system.

Also, the OpenDSS files (.dss) for these networks and some references works can be found here: (https://www.dropbox.com/sh/15fo64kxz7115e2/AAAGgAVzJmnLzA6rVlqfBdGca?dl=0)

## 2. Article Matlab code
[**"IBE MatlabCode"**](https://github.com/Zecao/IterativeBranchExchange/tree/master/IBE_MatlabCode)
I'll publish the Matlab code for replication of all the results from article, **as soon the article be published**.

## 3. Article OpenDSS customization code
[**"Executor OpenDSS"**](https://github.com/Zecao/ExecutorOpenDssBr)
Separate repository with detailed explanations for the OpenDSS customization C# code. 

## 4. OpenDSS Cemig-D Distribution feeders
[**"CemigFeeders"**](https://github.com/Zecao/2020Dijkstra/tree/master/CemigDFeeders)
Directory with the OpenDSS files (.dss) from 2 Cemig distribution feeders and 2 substations. These files are public as they were generated from a geographic distribution data base BDGD from Brazilian Regulatory Agency ANEEL.

## 5. 
Spreadsheet detailing all the reconfigurations implemented in [**Cemig-D**](https://www.cemig.com.br/en/) feeders using the Iterative Branch Exchange. 

Ezequiel C. Pereira
