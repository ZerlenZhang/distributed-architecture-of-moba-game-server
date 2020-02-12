namespace ReadyGamerOne.Data
{
	public interface IDataFactory
	{
		ITxtSerializable CreateData(string initLine);
	}
}

