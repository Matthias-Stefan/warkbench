#pragma once

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

struct image 
{
    uint32_t width;
    uint32_t height;
    uint32_t format;
    uint64_t data_size;
    uint8_t* data;
};

DLL_EXPORT  bool load_image(const char* path, image* out_image);

DLL_EXPORT  void free_image(image* image);

#ifdef __cplusplus
}
#endif