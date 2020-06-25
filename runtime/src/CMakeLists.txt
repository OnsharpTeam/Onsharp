set(PLUGIN_CONFIG_INCLUDE_DIR "${CMAKE_CURRENT_BINARY_DIR}/config_headers")
link_libraries(stdc++fs)

configure_file(
        "version.hpp.in"
        "${PLUGIN_CONFIG_INCLUDE_DIR}/version.hpp"
        @ONLY
)

add_library(OnsharpRuntime MODULE
        Plugin.cpp
        Plugin.hpp
        Singleton.hpp
        PluginInterface.cpp)

target_include_directories(OnsharpRuntime PRIVATE
        ${PLUGIN_CONFIG_INCLUDE_DIR}
        ${HORIZONSDK_INCLUDE_DIR}
        ${JNI_INCLUDE_DIRS}
        )

set_property(TARGET OnsharpRuntime PROPERTY PREFIX "")

set_property(TARGET OnsharpRuntime PROPERTY CXX_STANDARD 17)
set_property(TARGET OnsharpRuntime PROPERTY CXX_STANDARD_REQUIRED ON)

if(MSVC)
    target_compile_definitions(OnsharpRuntime PRIVATE
            _CRT_SECURE_NO_WARNINGS
            NOMINMAX
            )
endif()

# increase warning levels
if (MSVC)
    target_compile_options(OnsharpRuntime PRIVATE
            /W4
            /WX
            /permissive-
            /wd4505
            )
elseif(UNIX)
    target_compile_options(OnsharpRuntime PRIVATE
            -Wall
            -Wextra
            -pedantic
            -Werror
            -Wno-unused-variable
            -Wno-unused-function
            )
endif()

target_link_libraries(OnsharpRuntime ${HORIZONSDK_LIBRARY})

if(UNIX)
    if(NOT APPLE)
        target_link_libraries(OnsharpRuntime rt)
    endif()

    set_target_properties(OnsharpRuntime PROPERTIES
            INSTALL_RPATH "$ORIGIN/..:$ORIGIN/")
endif()

install(TARGETS OnsharpRuntime DESTINATION "plugins/")

set(CPACK_PACKAGE_VERSION ${PROJECT_VERSION})
if(WIN32)
    set(CPACK_GENERATOR ZIP)
else()
    set(CPACK_GENERATOR TGZ)
endif()

include(CPack)