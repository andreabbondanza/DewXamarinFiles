# DewXamarinFiles
A library for xamarin forms to manage files and local settings.

## How to use

This is a static class, so you can use the methods and attributes directly.

### Files

Here the methods/attributes that help you to manage files.

#### Attributes

- __BufferSize__: _int_ - Is the buffer size for read/write files

#### Methods

- __ReadLocalFileAsync__: (__path__: _string_ : _The path to the file_) : _return_ __byte[]__
- __ReadLocalFileTextAsync__: (__path__: _string_ : _The path to the file_) : _return_ __string__
- __WriteLocalFileAsync__: (_s_: _byte[]_ : The data to write, __name__: _string_ : _filename_, __path__: _string_ : _the path to the folder_)
- __WriteLocalFileAsync__: (_s_: _string_ : The data to write, __name__: _string_ : _filename_, __path__: _string_ : _the path to the folder_)
- __ApplicationPath__: _return_ __string__ : _the path to the local app folder with write permissions__
- __CheckFileExists__: (__file__: _string_ : __file path__) : _return_ __bool__
- __CheckDirectoryExists__: (__path__: _string_ : _directory path_) _return_ __bool__

#### Example

```csharp
using DewCore.Xamarin.Files;
using DewCore.Extensions.Strings;
public class Example
{
    public async Task CreateFileWithText(string toWrite)
    {
        var basePath = DewXamarinFiles.ApplicationPath();
        if(!DewXamarinFiles.CheckFileExists(basePath+Path.DirectorySeparator+"textfile.txt"))
            await DewXamarinFiles.WriteLocalFileTextAsync(towrite,"textfile.txt",basePath);
        if(!DewXamarinFiles.CheckFileExists(basePath+Path.DirectorySeparator+"bytefile"))
            await DewXamarinFiles.WriteLocalFileTextAsync(towrite.ToBytes(),"bytefile",basePath);
        string text = await DewXamarinFiles.ReadLocalFileTextAsync(basePath+Path.DirectorySeparator+"textfile.txt");
        byte[] bytes = await DewXamarinFiles.ReadLocalFileAsync(basePath+Path.DirectorySeparator+"textfile.txt");
    }
}
```
#### NOTE
In app environemnt, you need to pass always the _basePath_ because you don't have the permissions to write wherever you want. The methods supports absolute paths just for the future, in the eventually of MacOS, Linux, PC xamarin forms supports.

### Local Settings

Here the methods/attributes that help you to manage local settings.

#### Attributes

- __SettingsName__: _string_ - Is the local settings file name (without extension), default value is "_\_\_dew\_loc\_set_"

#### Methods

- __WriteLocalSettings__: (__string__: _key_ : _local setting key_, __string__: _option_ : _local setting value_)
- __WriteLocalSettings\<T\>__: (__string__: _key_ : _local setting's key_, __T__: _option_ : _local setting value_) - T must implmenent a ToString method to serialize
- __CheckLocalSettingExists__: (__string__: _key_ : _local setting's key_) : _return_ __bool__
- __ReadLocalSetting__: (__string__: _key_ : _local setting's key_): _return_ __string__
- __ReadLocalSetting\<T\>__: (__string__: _key_ : _local setting's key_): _return_ __T__
- __SerializeObject\<T\>__: (__T__ toSerialize : _Object to serialize_): _return_ __string__
- __DeserializeObject\<T\>__: (__string__ toDeserialize : _Object to deserialize_): _return_ __T__


#### Example

```csharp
using DewCore.Xamarin.Files;
using DewCore.Extensions.Strings;
public class Example
{
    public class User
    {
        public string Name {get;set;}
        public string Surname {get;set;}
    }
//-----------------------------------------------------------------------
    public async Task<bool> WriteToken()
    {
        if(!(await DewXamarinFiles.CheckLocalSettingExists("token")))
            return false;
        var token = await DewXamarinFiles.ReadLocalSettings("token");
        if(token.Length < 10)
            while(token.Length < 10)
                token += "0";
        await DewXamarinFiles.WriteLocalSetting("token",token);
        return true;
    }
//-----------------------------------------------------------------------
    public async Task WriteUser(User u)
    {
        await DewXamarinFiles.WriteLocalSetting<User>("user",u);    
    }
//-----------------------------------------------------------------------
    public async Task<User> ReadUser()
    {
        if(!(await DewXamarinFiles.CheckLocalSettingExists("user")))
            return null;
        return DewXamarinFiles.ReadLocalSetting<User>("user");
    }
//-----------------------------------------------------------------------
}
```

### Note

- Type __T__ must be serializable
- When you try to read a local setting and the file doesn't exist, it will throw a __FileNotFoundException__, so Use _CheckLocalSettingExists_ before
- When you call __CheckLocalSettings__, if the file doesn't exist it will be created with "{}" object into.
- When you call a __Write__ function, if the file doesn't exist it will be created with "{}" object into.

## About
[Andrea Vincenzo Abbondanza](http://www.andrewdev.eu)

## Donate
[Help me to grow up, if you want](https://payPal.me/andreabbondanza)
