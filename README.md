This project reads csv files and processes its records according to particular specifications.
The aim is to process big files (1GB and more) without storing any values in collections otherwise we could run out of memory.
The only disadvantage using this approach would be that the process has to read each file in two steps (cycles).
The first cycle reads the data and calculates the average values or something else at the end of the first cycle.
The second cycle iterates through the file again and picks up the records which pass some criteria.

If we need to implement a new processor then we just have to add a new processor to the Processors project,
e.g. class "Proc_Lp" shows how to implement the whole functionality. Btw. this class also contains static method
Get_Data_Directly which is used only for unit testing and this approach shows the other scenario - instead of
reading the csv file twice we read it only once, store all necessary values into a collection and then process
the data at the end.