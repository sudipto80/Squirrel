Example #7 (Calculating speed of a bungee jumper)
==============

<img src ="http://www.coupondunia.in/blog/wp-content/uploads/2013/12/Bungee_Jumping-1-1050x591.jpeg"/>

Calculating the speed of a bungee jumper as a function of time is goverened by the following formula 

<img src="https://pbs.twimg.com/media/CGb_sywUQAAIk2z.png" border="0"/>

where 
```g``` is the gravitational constant 9.81 m/s^2 

```Cd``` is the drag of the air 

```m``` is the mass of the jumper 

```t``` is time measured in seconds. 

The following code calculates the speed of a jumper whose weight is 68.1 Kg and for an air drag of 0.25
tanh() or hyperbolic tan can be caclualted from the following formula. I found this from this [helpful reference]( 
http://math2.org/math/trig/hyperbolics.htm) 

<img src="https://pbs.twimg.com/media/CGcAU-1UgAAk_2E.png" border="0" />

NCalc has the function ```Exp``` to calculate exponentiation. 

```csharp 
Table bungee = new Table();

bungee.AddColumn("Time", Enumerable.Range(1, 60).Select(x => x.ToString()).ToList());
//Adding a column called "X" to facilitate the calculation
bungee.AddColumn("X", "Sqrt(9.81*0.25/68.1)*[Time]", 4);
//Adding a column "Speed" to hold the values of speed at any given time
bungee.AddColumn("Speed", @"Sqrt (9.81*68.1/0.25) * (Exp([X])-Exp(-[X]))/(Exp([X])+Exp(-[X]))", 5);
            
//Creating a "column" plot of this data
string html = bungee
                .Pick("Time","Speed")
                .ToBarChartByGoogleDataVisualization("Time",
                                                     "Speed at a given time",
                                                     "Speed of bungee jumper",
                                                     GoogleDataVisualizationcs.BarChartType.Column);
//Writing the result to a html file            
StreamWriter htmlWriter = new StreamWriter("temp.htm");
htmlWriter.WriteLine(html);
htmlWriter.Close();

//Launching the browser to see the plot
System.Diagnostics.Process.Start("temp.htm");
```

This produces the following plot 

<!--<img src="http://gifyu.com/images/bungeeop.png" alt="bungeeop.png" border="0" />-->
<img src="bungee.gif" />

From this plot we can see that the jumper attained his terminal speed within the first minute. 
