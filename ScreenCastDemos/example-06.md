Example #6 (Titanic Survivor Statistics)
====================
First few rows of the titanic dataset looks like this 

<img src="http://gifyu.com/images/titanic_survival.png" border="0">

Survived "0" means the person didn't survive and "1" means that the person survived. THe question posed is what is the percentage of people in each class that survived and died. "Pclass" column depicts the class of the person boarded. "1" means first class, "2" means second class and "3" means third class. 

The following code uses Squirrel to solve it. 
```csharp
Table titanic = DataAcquisition.LoadCSV(@"titanic.csv");
var classes = titanic.SplitOn("Pclass");
Table result = new Table ();
for (int pClass = 1; pClass <= 3;pClass++ )
{
    Dictionary<string,string> thisRow = new Dictionary<string,string> ();
    thisRow.Add("Class",pClass.ToString());
    thisRow.Add("Died(%)", (100 * classes[pClass.ToString()].GetPercentage("Survived", "0")).ToString());
    thisRow.Add("Survived(%)", (100 * classes[pClass.ToString()].GetPercentage("Survived", "1")).ToString());
    result.AddRow(thisRow);
}
result.PrettyDump();
```
This produces the following output 

<img src="http://gifyu.com/images/titanic_survival_op.png" border="0">

This uses the function ```GetPercentage``` which returns the percentage of values of the given column that match with the given value. For example here it returns the percentage of values in "Survived" column that is "0" for calculating "Died(%)" column. 
