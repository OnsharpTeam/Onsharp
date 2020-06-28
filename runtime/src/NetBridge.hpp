#pragma once
#ifndef __NET_BRIDGE_H__
#define __NET_BRIDGE_H__
#include <string>
#include <stdio.h>
#ifdef _WIN32
#include <direct.h>
#define GetCurrentDir _getcwd
#else
#include <unistd.h>
#define GetCurrentDir getcwd
#endif

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <string>

#include "coreclrhost.h"

#define NET_NO_ERROR 0
#define NET_CONSOLE_ERROR -1
#define NET_SUCCESS 1

#if defined(_WIN32)
#include <Windows.h>
#define FS_SEPARATOR "\\"
#define PATH_DELIMITER ";"
#define CORECLR_FILE_NAME "coreclr.dll"
#elif defined(__linux__)
#include <dirent.h>
#include <dlfcn.h>
#include <limits.h>
#define FS_SEPARATOR "/"
#define PATH_DELIMITER ":"
#define MAX_PATH PATH_MAX
#if OSX
#define CORECLR_FILE_NAME "libcoreclr.dylib"
#else
#define CORECLR_FILE_NAME "libcoreclr.so"
#endif
#endif

typedef int (*report_callback_ptr)(int progress);
typedef void (*load_ptr)(const char* appPath);
typedef void (*unload_ptr)();
typedef bool (*execute_event_ptr)(const char* name, const char* data);

#ifdef __cplusplus
extern "C"
{
#endif

class NetBridge
{
private:
    void* hostHandle;
    unsigned int domainId;
    coreclr_initialize_ptr initializeCoreClr;
    coreclr_create_delegate_ptr createManagedDelegate;
    coreclr_shutdown_ptr shutdownCoreClr;
    execute_event_ptr executeEvent;
    unload_ptr unload;

public:
    int last_error = NET_NO_ERROR;

    NetBridge()
    {
        char aCurrentPath[FILENAME_MAX];

        if (!GetCurrentDir(aCurrentPath, sizeof(aCurrentPath)))
        {
            printf("ERROR: No app path found!");
            last_error = NET_CONSOLE_ERROR;
            return;
        }

        aCurrentPath[sizeof(aCurrentPath) - 1] = '\0';
        std::string appPath = std::string(aCurrentPath);

        char gCurrentPath[FILENAME_MAX];

        if (!GetCurrentDir(gCurrentPath, sizeof(gCurrentPath)))
        {
            printf("ERROR: No coreclr path found!");
            last_error = NET_CONSOLE_ERROR;
            return;
        }

        gCurrentPath[sizeof(gCurrentPath) - 1] = '\0';
#if defined(_WIN32)
        std::string coreClrPath = std::string(gCurrentPath) + "\\onsharp\\runtime\\coreclr.dll";
#else
        std::string coreClrPath = std::string(gCurrentPath) + "\\onsharp\\runtime\\libcoreclr.so";
#endif
        char cCurrentPath[FILENAME_MAX];

        if (!GetCurrentDir(cCurrentPath, sizeof(cCurrentPath)))
        {
            printf("ERROR: No Wrapper path found!");
            last_error = NET_CONSOLE_ERROR;
            return;
        }

        cCurrentPath[sizeof(cCurrentPath) - 1] = '\0';
#if defined(_WIN32)
        std::string wrapperPath = std::string(cCurrentPath) + "\\onsharp\\runtime\\Onsharp.dll";
#else
        std::string wrapperPath = std::string(cCurrentPath) + "\\onsharp\\runtime\\Onsharp.dll";
#endif

        char rCurrentPath[FILENAME_MAX];

        if (!GetCurrentDir(rCurrentPath, sizeof(rCurrentPath)))
        {
            printf("ERROR: No Runtime path found!");
            last_error = NET_CONSOLE_ERROR;
            return;
        }

        rCurrentPath[sizeof(rCurrentPath) - 1] = '\0';
#if defined(_WIN32)
        std::string runtimePath = std::string(rCurrentPath) + "\\onsharp\\runtime\\";
#else
        std::string runtimePath = std::string(rCurrentPath) + "\\onsharp\\runtime\\";
#endif

#if defined(_WIN32)
        HMODULE coreClr = LoadLibraryExA(coreClrPath.c_str(), NULL, 0);
#elif defined(__linux__)
        void* coreClr = dlopen(coreClrPath.c_str(), RTLD_NOW | RTLD_LOCAL);
#endif
        if (coreClr == NULL)
        {
            printf("ERROR: Failed to load CoreCLR from %s\n", coreClrPath.c_str());
            last_error = NET_CONSOLE_ERROR;
            return;
        }

#if defined(_WIN32)
        initializeCoreClr = (coreclr_initialize_ptr)GetProcAddress(coreClr, "coreclr_initialize");
        createManagedDelegate = (coreclr_create_delegate_ptr)GetProcAddress(coreClr, "coreclr_create_delegate");
        shutdownCoreClr = (coreclr_shutdown_ptr)GetProcAddress(coreClr, "coreclr_shutdown");
#elif defined(__linux__)
        initializeCoreClr = (coreclr_initialize_ptr)dlsym(coreClr, "coreclr_initialize");
		createManagedDelegate = (coreclr_create_delegate_ptr)dlsym(coreClr, "coreclr_create_delegate");
		shutdownCoreClr = (coreclr_shutdown_ptr)dlsym(coreClr, "coreclr_shutdown");
#endif

        if (initializeCoreClr == NULL)
        {
            printf("ERROR: coreclr_initialize not found");
            last_error = NET_CONSOLE_ERROR;
            return;
        }

        if (createManagedDelegate == NULL)
        {
            printf("ERROR: coreclr_create_delegate not found");
            last_error = NET_CONSOLE_ERROR;
            return;
        }

        if (shutdownCoreClr == NULL)
        {
            printf("ERROR: coreclr_shutdown not found");
            last_error = NET_CONSOLE_ERROR;
            return;
        }

        std::string tpaList;

#if defined(_WIN32)
        std::string searchPath(runtimePath.c_str());
        searchPath.append(FS_SEPARATOR);
        searchPath.append("*");
        searchPath.append(".dll");

        WIN32_FIND_DATAA findData;
        HANDLE fileHandle = FindFirstFileA(searchPath.c_str(), &findData);

        if (fileHandle != INVALID_HANDLE_VALUE)
        {
            do
            {
                tpaList.append(runtimePath.c_str());
                tpaList.append(FS_SEPARATOR);
                tpaList.append(findData.cFileName);
                tpaList.append(PATH_DELIMITER);
            } while (FindNextFileA(fileHandle, &findData));
            FindClose(fileHandle);
        }
#elif defined(__linux__)
        DIR* dir = opendir(runtimePath.c_str());
			struct dirent* entry;
			int extLength = strlen(".dll");

			while ((entry = readdir(dir)) != NULL)
			{
				std::string filename(entry->d_name);
				int extPos = filename.length() - extLength;
				if (extPos <= 0 || filename.compare(extPos, extLength, ".dll") != 0)
				{
					continue;
				}
				tpaList.append(runtimePath.c_str());
				tpaList.append(FS_SEPARATOR);
				tpaList.append(filename);
				tpaList.append(PATH_DELIMITER);
			}
#endif
        const char* propertyKeys[] = {
                "TRUSTED_PLATFORM_ASSEMBLIES"
        };

        const char* propertyValues[] = {
                tpaList.c_str()
        };

        int hr = initializeCoreClr(
                wrapperPath.c_str(),        // App base path
                "Onsharp",       // AppDomain friendly name
                sizeof(propertyKeys) / sizeof(char*),   // Property count
                propertyKeys,       // Property names
                propertyValues,     // Property values
                &hostHandle,        // Host handle
                &domainId);         // AppDomain ID

        if (hr < 0)
        {
            printf("ERROR: coreclr_initialize failed - status: 0x%08x\n", hr);
            last_error = NET_CONSOLE_ERROR;
            return;
        }

        load_ptr managedDelegate;
        hr = createManagedDelegate(
                hostHandle,
                domainId,
                "Onsharp",
                "Onsharp.Native.Bridge",
                "Load",
                (void**)&managedDelegate);

        if (hr < 0)
        {
            printf("ERROR: load delegate failed - status: 0x%08x\n", hr);
            last_error = NET_CONSOLE_ERROR;
            return;
        }

        hr = createManagedDelegate(
                hostHandle,
                domainId,
                "Onsharp",
                "Onsharp.Native.Bridge",
                "Unload",
                (void**)&unload);

        if (hr < 0)
        {
            printf("ERROR: unload delegate failed - status: 0x%08x\n", hr);
            last_error = NET_CONSOLE_ERROR;
            return;
        }

        hr = createManagedDelegate(
                hostHandle,
                domainId,
                "Onsharp",
                "Onsharp.Native.Bridge",
                "ExecuteEvent",
                (void**)&executeEvent);

        if (hr < 0)
        {
            printf("ERROR: execute_event delegate failed - status: 0x%08x\n", hr);
            last_error = NET_CONSOLE_ERROR;
            return;
        }

        managedDelegate(appPath.c_str());
        last_error = NET_SUCCESS;
    }

    void stop()
    {
        unload();
        int hr = shutdownCoreClr(hostHandle, domainId);
        if (hr >= 0)
        {
            last_error = NET_SUCCESS;
        }
        else
        {
            printf("ERROR: coreclr_shutdown failed - status: 0x%08x\n", hr);
            last_error = NET_CONSOLE_ERROR;
        }
    }

    bool execute_event(const char* name, const char* data)
    {
        return executeEvent(name, data);
    }
};



#ifdef __cplusplus
}
#endif
#endif