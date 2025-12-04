#include "stb_image_lib_export.h"

#define STB_IMAGE_IMPLEMENTATION
#include "stb_image.h"

#include <stdint.h>
#include <cstring> 
#include <string>
#include <unordered_map>
#include <vector>

extern "C" {

DLL_EXPORT bool load_image(const char* path, image* out_image) 
{
    int width, height, channels;
    uint8_t* pixels = stbi_load(path, &width, &height, &channels, 4); // force RGBA

    if (!pixels)
    {
        return false;
    }

    out_image->width = (uint32_t)width;
    out_image->height = (uint32_t)height;
    out_image->format = 0;
    out_image->data_size = out_image->width * out_image->height * 4; 
    out_image->data = pixels;

    return true;
}

DLL_EXPORT void free_image(image* image)
{
    delete[] image->data;

    image->width = 0;
    image->height = 0;
    image->format = 0;
    image->data_size = 0;
    image->data = nullptr;
}

} // extern "C"