#include "tiny_obj_loader_lib_export.h"

#include "obj_vertex.h"
#include "obj_vertex_hash.h"

#define TINYOBJLOADER_IMPLEMENTATION
#include "tiny_obj_loader.h"

#include <stdint.h>
#include <cstring> 
#include <string>
#include <unordered_map>
#include <vector>

extern "C" {

DLL_EXPORT bool load_obj(const char* path, obj_mesh* out_mesh) 
{
    std::vector<obj_vertex> vertices{};
    std::vector<uint32_t> indices{};

    tinyobj::attrib_t attrib;
    std::vector<tinyobj::shape_t> shapes;
    std::vector<tinyobj::material_t> materials;
    std::string warn, err;

    if (!tinyobj::LoadObj(&attrib, &shapes, &materials, &warn, &err, path)) 
    {
        throw std::runtime_error(warn + err);
    }

    std::unordered_map<obj_vertex, uint32_t> unique_vertices;

    for (const auto& shape : shapes) 
    {
        for (const auto& index : shape.mesh.indices) 
        {
            obj_vertex vertex{};

            vertex.position[0] = attrib.vertices[3 * index.vertex_index + 0];
            vertex.position[1] = attrib.vertices[3 * index.vertex_index + 1];
            vertex.position[2] = attrib.vertices[3 * index.vertex_index + 2];

            vertex.normal[0] = attrib.normals[3 * index.normal_index + 0];
            vertex.normal[1] = attrib.normals[3 * index.normal_index + 1];
            vertex.normal[2] = attrib.normals[3 * index.normal_index + 2];
                
            vertex.uv[0] = attrib.texcoords[2 * index.texcoord_index + 0];
            vertex.uv[1] = 1.0f - attrib.texcoords[2 * index.texcoord_index + 1];

            if (unique_vertices.count(vertex) == 0) 
            {
                unique_vertices[vertex] = static_cast<uint32_t>(vertices.size());
                vertices.push_back(vertex);
            }

            indices.push_back(unique_vertices[vertex]);
        }
    }

    out_mesh->vertex_count = static_cast<int>(vertices.size());
    out_mesh->vertices = new obj_vertex[out_mesh->vertex_count];
    memcpy(out_mesh->vertices, vertices.data(), sizeof(obj_vertex) * out_mesh->vertex_count);

    out_mesh->index_count = static_cast<int>(indices.size());
    out_mesh->indices = new uint32_t[out_mesh->index_count];
    memcpy(out_mesh->indices, indices.data(), sizeof(uint32_t) * out_mesh->index_count);

    return true;
}

DLL_EXPORT void free_obj_mesh(obj_mesh* mesh)
{
    if (!mesh) 
    {
        return;
    }

    delete[] mesh->vertices;
    delete[] mesh->indices;

    mesh->vertices = nullptr;
    mesh->indices = nullptr;
    mesh->vertex_count = 0;
    mesh->index_count = 0;
}

} // extern "C"