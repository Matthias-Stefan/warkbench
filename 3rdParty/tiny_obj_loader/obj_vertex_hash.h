#pragma once

#include "obj_vertex.h"

#include <functional>
#include <cstddef>
#include <cstdint>

inline bool operator==(const obj_vertex& a, const obj_vertex& b)
{
    return a.position[0] == b.position[0] &&
           a.position[1] == b.position[1] &&
           a.position[2] == b.position[2] &&
           a.normal[0] == b.normal[0] &&
           a.normal[1] == b.normal[1] &&
           a.normal[2] == b.normal[2] &&
           a.uv[0] == b.uv[0] &&
           a.uv[1] == b.uv[1];
}

namespace std 
{
    template<>
    struct hash<obj_vertex> 
    {
        size_t operator()(const obj_vertex& v) const noexcept 
        {
            auto hashf = [](float f) -> size_t 
            {
                uint32_t bits = *reinterpret_cast<const uint32_t*>(&f);
                return std::hash<uint32_t>()(bits);
            };

            size_t h = 0;
            h ^= hashf(v.position[0]) + 0x9e3779b9 + (h << 6) + (h >> 2);
            h ^= hashf(v.position[1]) + 0x9e3779b9 + (h << 6) + (h >> 2);
            h ^= hashf(v.position[2]) + 0x9e3779b9 + (h << 6) + (h >> 2);
            h ^= hashf(v.normal[0]) + 0x9e3779b9 + (h << 6) + (h >> 2);
            h ^= hashf(v.normal[1]) + 0x9e3779b9 + (h << 6) + (h >> 2);
            h ^= hashf(v.normal[2]) + 0x9e3779b9 + (h << 6) + (h >> 2);
            h ^= hashf(v.uv[0]) + 0x9e3779b9 + (h << 6) + (h >> 2);
            h ^= hashf(v.uv[1]) + 0x9e3779b9 + (h << 6) + (h >> 2);
            return h;
        }
    };
}
