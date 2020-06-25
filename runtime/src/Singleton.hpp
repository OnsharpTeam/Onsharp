/*
Copyright (C) 2019 Blue Mountains GmbH
This program is free software: you can redistribute it and/or modify it under the terms of the Onset
Open Source License as published by Blue Mountains GmbH.
This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without
even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
See the Onset Open Source License for more details.
You should have received a copy of the Onset Open Source License along with this program. If not,
see https://bluemountains.io/Onset_OpenSourceSoftware_License.txt
*/

#pragma once


template<class T>
class Singleton
{
protected:
    static T *m_Instance;

public:
    Singleton()
    { }
    virtual ~Singleton()
    { }

    inline static T *Get()
    {
        if (m_Instance == nullptr)
            m_Instance = new T;
        return m_Instance;
    }

    inline static void Destroy()
    {
        if (m_Instance != nullptr)
        {
            delete m_Instance;
            m_Instance = nullptr;
        }
    }
};

template <class T>
T* Singleton<T>::m_Instance = nullptr;