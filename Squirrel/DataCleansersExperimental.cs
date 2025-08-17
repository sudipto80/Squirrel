using Squirrel;

namespace Squirrel.DataCleansing;

public static class DataCleansersExperimental
{
    
		/// <summary>
		/// Automatically cleans a table instance 
		/// </summary>
		/// <param name="tab"></param>
		/// <returns></returns>
		public static Table AutoClean(this Table tab)
		{
			//Find bad values from teach column 
			//Remove bad values from each column 
			//Return the cleaned table 
			
			//if the character set used in a value of a cell in a column
			//is drastically different from the rest of the values of rest of the cells 
			//then it is probably a bad value 
			
			//Once all the bad values from each of the column is identified, we need to put them in a dictionary
			//With column names and bad values and then we need to iterte over this dictionary to clean the data
			//for each column. 
			
			var cleaned = new Table();
			return cleaned;
		}
		/// <summary>
		/// Auto Clean an entire column
		/// </summary>
		/// <param name="tab"></param>
		/// <param name="column"></param>
		/// <returns></returns>
		public static Table AutoClean(this Table tab, string column)
		{
			var colValues = tab.ValuesOf(column);
			
			Table cleaned = new Table();
			return cleaned;
		}
		
		/// <summary>
		/// A way to find out which Normalization strategy will work best for this 
		/// particular column values. 
		/// </summary>
		/// <param name="tab"></param>
		/// <param name="columnName"></param>
		/// <returns></returns>
		public static NormalizationStrategy InferRequiredStrategy(this Table tab, string columnName)
		
		{
			//If the values of the cells are less than 5 characters long,
			//then they are probably hard to pronounce and are possible acronyms.
			//For these sorts of values, UpperCase may be the best option. 
			return NormalizationStrategy.SentenceCase;
		} 
		
}