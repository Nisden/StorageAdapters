#Usage

The primary purpose is to provide a common interface for accessing diffrent types of storage.
All the diffrent Storage Services implement a minumn amount of features and then the Storage Adapter fakes the rest.

```csharp
var storageAdapter = new IOStorageAdapter(new IOStorageConfiguration() { BaseDirectory = "C:\\" });
await storageAdapter.GetDirectories("Program Files"); // List the directories in C:\Program Files
await storageAdapter.GetDirectoriesRecursive("Program Files"); // List the directories and sub directories in C:\Program Files (This is a faked method)
```

All methods except changing the configuration should be thread safe.

# Supported platforms

| Project                                                          | .NET 4.5     | Windows Store 8  | Windows Store 8.1 | Windows Phone Store 8.1 | Windows Universal 10.0
| -----------------------------------------------------------------|--------------|------------------|-------------------|-------------------------|----------------------------
| StorageAdapters                                                  | ✓            | ✓               | ✓                 | ✓                       | ✓
| StorageAdapters.InMemory                                         | ✓            | ✓               | ✓                 | ✓                       | ✓
| StorageAdapters.WebDAV                                           | ✓            | ✓               | ✓                 | ✓                       | ✓
| [StorageAdapters.BackBlaze](https://www.backblaze.com/b2/docs/)  | ✓            | ✓               | ✓                 | ✓                       | ✓
| StorageAdapters.Azure                                            | ✓            |                  | ✓                 | ✓                      | ✓
| StorageAdapters.FTP (WIP)                                        | (✓)          |                  | ✓                 | ✓                      | ✓
| StorageAdapters.IO                                               | ✓            |                  |                   |                         |

(✓) Not implemented yet

## Limitations

* **WebDAV** can not have files without extensions.
* **WebDAV** deletes are always recursive.
* **Azure** sub directories always exist
* **Azure** you can only delete a directory when it contains files
* **IO** can not have a filename longer then 255 characters.


# Structure and Responsiblities


* **IStorageService**
Implementation of a storage service, it handles all the basic operations and communication with the underlying storage system.
* **IStorageConfiguration**
The basic configuration values that must be defined
* **IStorageAdapter**
Sanitizes input before the IStorageService executes it and handles the logic that is expected by clients, like creating folders before uploading documents and such.

## Base Implementations and Genrics


| Interface         | Class                 | Description
| ------------------|-----------------------|-----------------------------------
| IStorageAdapter   | StorageAdapterBase    | Handles a lot of logic and is the recommended base class
| IStorageService   | HTTPStorageServiceBase| Handles logic common for HTTP based services (Cookies, request logging, etc)
| IVirtualDirectory | GenericDirectory      | Just a placeholder
| IVirtualFileInfo  | GenericFileInfo       | Just another placeholder

## Creating a new StorageAdapter

To create a new storage adapter implement the following:

1. Create a new instance of **IStorageConfiguration** defining properties for authentication and starting paths
2. Create a new instance of **IStorageService** with the new configuration as your new **IStorageConfiguration**
⋅⋅⋅ You must implement all the methods for the StorageAdapter to be officialy adopted
3. Create a new instance of **IStorageAdapter** by inheriting from **StorageAdapterBase**
⋅⋅⋅ If your StorageService implements "shortcuts" or optimized methods like Recursive Deletes, overwrite the default delete method in **StorageAdapterBase** with a call to the optimized method on your StorageService

## Testing

All tests are currently done on the implemented **StorageAdapterBase**, there is a generic **StorageAdapterTest** that you can implement in your test project that will provide you the basics of all normally supported functions.

It is recommended that if you have any "special" logic or such that you implement additional tests for this.

## Requirements

* Visual Studio 2015
* Windows 8.1 or newer (Only for the WindowsStore targets)