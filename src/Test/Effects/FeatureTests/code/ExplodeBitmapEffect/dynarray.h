// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*
    A simple dynamic array 
*/

#pragma once

#include <vector>

template<typename T> class DynArray
{
public:
    DynArray()
    {
    }
    ~DynArray()
    {
    }

    UINT GetCount() const
    {
        return (UINT)m_vector.size();
    }

    void Set( INT n, T t )
    {
        m_vector[n] = t;
    }

    T Get( INT n ) const
    {
        return m_vector[n];
    }

    /*
    T operator[](INT n) const
    {
        return m_vector[n];
    }
    */

    HRESULT Add(__in_ecount(1) const T& newItem)
    {
        m_vector.push_back( newItem );
        return S_OK;
    }

    void Reset()
    {
        m_vector.clear();
    }

protected:
    std::vector<T> m_vector;
};


