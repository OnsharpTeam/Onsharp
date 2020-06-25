# Look for Horizon plugin sdk
# Once done, this will define
#
#  HORIZONSDK_FOUND - system has Horizon plugin SDK
#  HORIZONSDK_INCLUDE_DIR - Horizon plugin SDK include directories
#  HORIZONSDK_LIBRARY - Horizon plugin SDK libraries
#
# The user may wish to set, in the CMake GUI or otherwise, this variable:
#  HORIZONSDK_ROOT_DIR - path to start searching for the module

set(HORIZONSDK_ROOT_DIR
        "${HORIZONSDK_ROOT_DIR}"
        CACHE
        PATH
        "Where to start looking for this component."
        )

find_path(
        HORIZONSDK_INCLUDE_DIR
        NAMES
        "PluginSDK.h"
        HINTS
        ${HORIZONSDK_ROOT_DIR}
        PATH_SUFFIXES
        include
)

if(WIN32)
    find_library(
            HORIZONSDK_LIBRARY
            NAMES
            "Lua.lib"
            HINTS
            ${HORIZONSDK_ROOT_DIR}
            PATH_SUFFIXES
            lib
    )
elseif(UNIX)
    find_library(
            HORIZONSDK_LIBRARY
            NAMES
            "libluaplugin.a"
            HINTS
            ${HORIZONSDK_ROOT_DIR}
            PATH_SUFFIXES
            lib
    )
else()
    message(FATAL_ERROR "Unsupported platform.")
endif()

mark_as_advanced(
        HORIZONSDK_INCLUDE_DIR
        HORIZONSDK_LIBRARY
)

include(FindPackageHandleStandardArgs)
find_package_handle_standard_args(
        HorizonPluginSDK
        REQUIRED_VARS
        HORIZONSDK_INCLUDE_DIR
        HORIZONSDK_LIBRARY
)

if(HORIZONSDK_FOUND)
    mark_as_advanced(HORIZONSDK_ROOT_DIR)
endif()