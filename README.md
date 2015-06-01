
Squirrel
======== 

<img src="http://gifyu.com/images/T-Shirt.png" border="0" height="250" width="300">

<!--<a href="Squirrel"><img src="https://raw.github.com/sudipto80/Squirrel/newb/img/icon_26718.png" align="left" t="100" width="100" ></a>-->

[`Squirrel by Pirog tetyana from The Noun Project`](https://raw.github.com/sudipto80/Squirrel/newb/img/license.txt)

Why Squirrel
------------
Data Analytics and Big Data, are now the buzz words of the industry. Today many Businesses want to drive their businesses using Data Analytics – by gaining insights from their data. Aesthetically pleasing data visualizations with agility are key for effective discovery of insights. And better insight requires a bunch of special skills – an expertise in the field of [Data Science](http://en.wikipedia.org/wiki/Data_science). But are Data Scientists easy to come by?
Most of the datasets used by the businesses are not anywhere near Big. Actually they are Tiny to Medium. A typical dataset has only few thousand rows! Professor [Alex Smola](https://www.linkedin.com/in/smola) has named datasets based on their sizes as follows:

|Dataset Size| Name |
:------------|:------|
|Dataset that can fit in your mobile phone |Tiny |
|Dataset that can fit in your laptop (1GB) |Small |
|Dataset that can fit in your workstation (4 GB) |Medium |
|Dataset that can fit in your server Large When clusters of your servers struggle| Big|


Businesses are so sold up to the idea of Big data (it has almost become a status symbol) that they ignore the power of small data tools developed in-house in deriving their insights. So could software developers replace the need of the Data Scientist in answering most questions that involve Tiny or Medium datasets?
Business users (Operational Managers, System Engineers, Financial and Planning Analysts, DevOps, etc.), often want to initiate the analysis and gain insight into the
business on their personal devices. But they cannot because of the special skills and training needed to perform such actions. Their outlook thus remains limited by the canned reports/insights offered by their backend. Could a Business user acquire, generate and visualize data of his choice, without knowing the details of data organization or query syntax?

**Squirrel** not only answers the above questions but also tries to fulfill the lack of a good data processing and analytics tool or framework to the large .NET developer ecosystem.

**Squirrel** seeks to simplify the task of discovering insights by bringing to the software developer a templatized design style for answering most common problems in data science. Squirrel’s readymade functions bring the agility in developing a solution or a storyline from any data.

**Squirrel** brings the application closer to the Business user by delivering the ability to acquire and visualize data from a variety of sources to their personal devices. We envision smart abilities in Squirrel that would bring agile data analytic solution development and delivery to near real time.

The **Squirrel** framework development is very active right now and early adopters can benefit by shaping up the design by requesting features. Squirrel is a simple and easy to use interface for querying and reporting of data. APIs for Data acquisition, Data filtering, Data cleansing, etc. provide simple solution, often in one step, for many real world problems. 

For a quick start take a look at the [CheatSheet] (http://www.slideshare.net/sudipto80/squirrel-do-morewithlesscodelightcheatsheet)

Data analytics solution development using **Squirrel** follows a templatized design style. As a Data Scientist would, a software developer using Squirrel too would solve a data analytics problem by stacking his solution starting with Data acquisition, followed by Data modeling & cleansing and then topping up with appropriate Data visualization. Applying Bootstrap to the visualization is automatic, bringing agility to development without compromising on quality of user experience. The following figure describes a stacked template of function blocks that aptly summarizes solution development using **Squirrel**

Here is a high level block diagram of all the components of Squirrel.
![Block Diagram](http://gifyu.com/images/blocks.png "High Level Block Diagram")


Key Design Philosophy
---------------------
Here are couple of design decisions that have been the guiding principle for the choice of internal data structure for Squirrel ```Table``` data structure to make data accessing/manipulating more intuitive and efficient at the same time.
* Each row in the table should be available by zero based integer indexing as we do in arrays and ```List<T>``` So if a table ```birthRates``` exists then we should be able to get to 10th row by the syntax ```birthRates[9]```
* A column at a given index should be available by the column name index. So if we have a table ```StockValues``` that stores average stock values in a year of different companies where the row depicts the year and the column depicts the company for which the stock price is stored, then we should be able to get the stock price for Microsoft (Symbol “MSFT”) for 5th year as ```StockValues[4][“MSFT”]```
* Value at row ```“k”``` (Expressed as an integer) and column ```“m”``` (Expressed as a string) has to be accessible by either of the syntax ```table[k][“m”]``` or ```table[“m”][k]```.


API Overview
------------

1. **BasicStatistics** - Basic statistical functions like Median, Range, Standard Deviation, Kurtosis, etc.
2. **CustomComparers** - Several customized comparators for sorting data.
3. **DataAcqusition** - Data loaded/dumped from/to various formats, e.g. CSV, TSV, HTML, ARFF, etc.
4. **DatabaseConnectors** - Data can be loaded from popular DB repositories by using the connectors for SQL Server and MongoDB.
5. **DataCleansers** - Extraction/Removal of outliers or data that matches specific boolean criteria.
6. **OrderedTable** - A data structure to hold sort results temporarily.
7. **Table** - An ubiquitous data structure used to encapsulate the data. Several APIs are part of the *Table* -
   * Filter data using regular expressions or SQL clause.
   * Sort data based on columns and their values.
   * Programmatic manipulation i.e. deletion, updation and insertion of data.
   * Merge data columns; Find subsets and exclusive or common rows in tables.
   * Other utilities to split or drop data columns; Find rows that meet a specific statistical critieria, e.g. top 10, below average, etc.
   * Natural queries

[Here](https://raw.github.com/sudipto80/Squirrel/newb/doc/TableAPI.chm) is the detailed API documentation.

Dependency
----------

There is a dependency for [NCalc](https://ncalc.codeplex.com/) for the following methods 
```csharp
AddColumn() 
AddRows()
AddRowsByShortHand()
``` 
NuGet Package
-------------
<img src="http://cdn.ws.citrix.com/wp-content/uploads/2011/09/NugetIcon.png"/>
You can integrate the package using NuGet by giving the following command</br>
```
PM> Install-Package TableAPI 
```
[Here is the NuGet Package page](https://www.nuget.org/packages/TableAPI/)

Documentation
-------------
[Here is a very high level list of functions and their summaries](https://github.com/sudipto80/Squirrel/blob/master/Documentations/Documentation.md). The documentation will be perpetually in-progress as the development is very active right now. Also this is a place where you can contribute. If you are looking for example, take a look at the documentation for [Aggregate](https://github.com/sudipto80/Squirrel/blob/master/Documentations/Aggregate.md) 

Running Squirrel on Ubuntu
---------------------------
You can use Squirrel on Linux/Ubuntu by installing [MonoDevelop](http://www.monodevelop.com/). Installing MonoDevelop installs all the required assemblies to run Squirrel. Here are the steps and they are not far from what would expect. In fact getting up to speed with Squirrel on Ubuntu is a breeze as you would see now. 

###Steps
1. Get MonoDevelop 
```sh
$ sudo apt-get install monodevelop
```
2. Step #2 You will be prompted to give your password.
3. Let it run and install monodevelop 
4. Once the installation is over, launch MonoDevelop by typing ```monodevelop``` in the console 
5. Download Squirrel by clicking on the "Download Zip" button 
6. Extract the content of this folder in a folder. I did it on desktop in a folder called "Squirrel".
7. Create a new project in Mono Develop and add the reference of TableAPI.dll and NCalc.dll 
8. Write the following code. It will work assuming you have saved Squirrel on desktop and the solution you created is named "HelloWorldSolution"

```csharp
using System;
using Squirrel;

namespace HelloWorldSolution
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Table iris = DataAcquisition.LoadCSV(@"/home/sudipta/Desktop/Squirrel/Squirrel-master/TableAPI/Data/iris.csv");
			iris.RandomSample(10)	
				.PrettyDump();
		}
	}
}
```

This will generate the following output

<img src="https://pbs.twimg.com/media/CGb0i9qUkAAaloi.png" border ="0"/>                                              

Some functionalities may not work on Ubuntu yet. If not, please report them by creating an issue. 

Examples
--------

1. [Do women pay more tip than men?](https://github.com/sudipto80/Squirrel/blob/master/ScreenCastDemos/example-01.md)
2. [Iris dataset aggregation](https://github.com/sudipto80/Squirrel/blob/master/ScreenCastDemos/example-02.md)
3. [Finding Gender-Ratio statistics in North America](https://github.com/sudipto80/Squirrel/blob/master/ScreenCastDemos/example-03.md)
4. [Finding top gold winning nations in Olympics](https://github.com/sudipto80/Squirrel/blob/master/ScreenCastDemos/example-04.md)
5. [How much money someone will accumulate at retirement](https://github.com/sudipto80/Squirrel/blob/master/ScreenCastDemos/example-05.md)
6. [Titanic Survivor Analysis per class](https://github.com/sudipto80/Squirrel/blob/master/ScreenCastDemos/example-06.md)
7. [Calculating speed of a bungee jumper](https://github.com/sudipto80/Squirrel/blob/master/ScreenCastDemos/example-07.md)

