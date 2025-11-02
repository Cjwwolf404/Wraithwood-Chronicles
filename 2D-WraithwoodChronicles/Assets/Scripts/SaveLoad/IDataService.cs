public interface IDataService
{
    bool SaveData<T>(string ReletivePath, T Data, bool Encrypted);

    T LoadData<T>(string ReletivePath, bool Encrypted);
}
