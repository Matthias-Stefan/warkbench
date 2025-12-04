#pragma once

#include "obj_vertex.h"

#include <stdint.h>
#include <functional>

#ifdef _WIN32
  #define DLL_EXPORT __declspec(dllexport)
#else
  #define DLL_EXPORT __attribute__((visibility("default")))
#endif


#ifdef __cplusplus
extern "C" {
#endif

struct obj_mesh 
{
    uint32_t vertex_count;
    uint32_t index_count;
    obj_vertex* vertices;
    uint32_t* indices;
};

DLL_EXPORT  bool load_obj(const char* path, obj_mesh* out_mesh);

DLL_EXPORT  void free_obj_mesh(obj_mesh* mesh);

#ifdef __cplusplus
}
#endif