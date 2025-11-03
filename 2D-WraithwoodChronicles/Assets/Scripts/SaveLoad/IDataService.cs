public interface IDataService
{
    bool SaveData<T>(string ReletivePath, T Data);

    T LoadData<T>(string ReletivePath);
}
