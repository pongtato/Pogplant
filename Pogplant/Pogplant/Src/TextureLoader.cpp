#include "TextureLoader.h"
#include "Logger.h"

#include <fstream>

#define STB_IMAGE_IMPLEMENTATION
#include <stb_image.h>

#define TINYDDSLOADER_IMPLEMENTATION
#include "tinyddsloader.h"
#include <glew.h>
#include <glfw3.h>

namespace Pogplant
{
    // Helpers for the tinyDDSloader
    struct GLSwizzle {
        GLenum m_r, m_g, m_b, m_a;
    };

    struct GLFormat {
        tinyddsloader::DDSFile::DXGIFormat m_dxgiFormat;
        GLenum m_type;
        GLenum m_format;
        GLSwizzle m_swizzle;
    };

    bool TranslateFormat(tinyddsloader::DDSFile::DXGIFormat fmt, GLFormat* outFormat) 
    {
        static const GLSwizzle sws[] = 
        {
            {GL_RED, GL_GREEN, GL_BLUE, GL_ALPHA},
            {GL_BLUE, GL_GREEN, GL_RED, GL_ALPHA},
            {GL_BLUE, GL_GREEN, GL_RED, GL_ONE},
            {GL_RED, GL_GREEN, GL_BLUE, GL_ONE},
            {GL_RED, GL_ZERO, GL_ZERO, GL_ZERO},
            {GL_RED, GL_GREEN, GL_ZERO, GL_ZERO},
        };
        using DXGIFmt = tinyddsloader::DDSFile::DXGIFormat;
        static const GLFormat formats[] = 
        {
            {DXGIFmt::R8G8B8A8_UNorm, GL_UNSIGNED_BYTE, GL_RGBA, sws[0]},
            {DXGIFmt::B8G8R8A8_UNorm, GL_UNSIGNED_BYTE, GL_RGBA, sws[1]},
            {DXGIFmt::B8G8R8X8_UNorm, GL_UNSIGNED_BYTE, GL_RGBA, sws[2]},
            {DXGIFmt::BC1_UNorm, 0, GL_COMPRESSED_RGBA_S3TC_DXT1_EXT, sws[0]},
            {DXGIFmt::BC2_UNorm, 0, GL_COMPRESSED_RGBA_S3TC_DXT3_EXT, sws[0]},
            {DXGIFmt::BC3_UNorm, 0, GL_COMPRESSED_RGBA_S3TC_DXT5_EXT, sws[0]},
            {DXGIFmt::BC4_UNorm, 0, GL_COMPRESSED_RED_RGTC1_EXT, sws[0]},
            {DXGIFmt::BC4_SNorm, 0, GL_COMPRESSED_SIGNED_RED_RGTC1_EXT, sws[0]},
            {DXGIFmt::BC5_UNorm, 0, GL_COMPRESSED_RED_GREEN_RGTC2_EXT, sws[0]},
            {DXGIFmt::BC5_SNorm, 0, GL_COMPRESSED_SIGNED_RED_GREEN_RGTC2_EXT,
             sws[0]},
        };
        for (const auto& format : formats) 
        {
            if (format.m_dxgiFormat == fmt) 
            {
                if (outFormat) 
                {
                    *outFormat = format;
                }
                return true;
            }
        }
        return false;
    }

    bool IsCompressed(GLenum fmt) 
    {
        switch (fmt) 
        {
        case GL_COMPRESSED_RGB_S3TC_DXT1_EXT:
        case GL_COMPRESSED_RGBA_S3TC_DXT1_EXT:
        case GL_COMPRESSED_RGBA_S3TC_DXT3_EXT:
        case GL_COMPRESSED_RGBA_S3TC_DXT5_EXT:
        case GL_COMPRESSED_RED_RGTC1_EXT:
        case GL_COMPRESSED_SIGNED_RED_RGTC1_EXT:
        case GL_COMPRESSED_RED_GREEN_RGTC2_EXT:
        case GL_COMPRESSED_SIGNED_RED_GREEN_RGTC2_EXT:
            return true;
        default:
            return false;
        }
    }

    bool LoadGLTexture(GLuint tex, tinyddsloader::DDSFile& dds) 
    {
        GLenum target = GL_INVALID_ENUM;
        bool isArray = false;
        if (dds.GetTextureDimension() == tinyddsloader::DDSFile::TextureDimension::Texture1D) 
        {
            if (dds.GetArraySize() > 1) 
            {
                target = GL_TEXTURE_1D_ARRAY;
                isArray = true;
            }
            else 
            {
                target = GL_TEXTURE_1D;
            }
        }
        else if (dds.GetTextureDimension() ==
            tinyddsloader::DDSFile::TextureDimension::Texture2D) 
        {
            if (dds.GetArraySize() > 1) 
            {
                if (dds.IsCubemap()) 
                {
                    if (dds.GetArraySize() > 6) 
                    {
                        target = GL_TEXTURE_CUBE_MAP_ARRAY;
                        isArray = true;
                    }
                    else 
                    {
                        target = GL_TEXTURE_CUBE_MAP;
                    }
                }
                else 
                {
                    target = GL_TEXTURE_2D_ARRAY;
                    isArray = true;
                }
            }
            else 
            {
                target = GL_TEXTURE_2D;
            }
        }
        else if (dds.GetTextureDimension() ==
            tinyddsloader::DDSFile::TextureDimension::Texture3D) 
        {
            target = GL_TEXTURE_3D;
        }

        GLFormat format;
        if (!TranslateFormat(dds.GetFormat(), &format)) 
        {
            return false;
        }

        glBindTexture(target, tex);
        glTexParameteri(target, GL_TEXTURE_BASE_LEVEL, 0);
        glTexParameteri(target, GL_TEXTURE_MAX_LEVEL, dds.GetMipCount() - 1);
        glTexParameteri(target, GL_TEXTURE_SWIZZLE_R, format.m_swizzle.m_r);
        glTexParameteri(target, GL_TEXTURE_SWIZZLE_G, format.m_swizzle.m_g);
        glTexParameteri(target, GL_TEXTURE_SWIZZLE_B, format.m_swizzle.m_b);
        glTexParameteri(target, GL_TEXTURE_SWIZZLE_A, format.m_swizzle.m_a);

        switch (target) 
        {
        case GL_TEXTURE_1D:
            glTexStorage1D(target, dds.GetMipCount(), format.m_format,
                dds.GetWidth());
            break;
        case GL_TEXTURE_1D_ARRAY:
            glTexStorage2D(target, dds.GetMipCount(), format.m_format,
                dds.GetWidth(), dds.GetArraySize());
            break;
        case GL_TEXTURE_2D:
            glTexStorage2D(target, dds.GetMipCount(), format.m_format,
                dds.GetWidth(), dds.GetHeight());
            break;
        case GL_TEXTURE_CUBE_MAP:
            glTexStorage2D(target, dds.GetMipCount(), format.m_format,
                dds.GetWidth(), dds.GetHeight());
            break;
        case GL_TEXTURE_2D_ARRAY:
            glTexStorage3D(target, dds.GetMipCount(), format.m_format,
                dds.GetWidth(), dds.GetHeight(), dds.GetArraySize());
            break;
        case GL_TEXTURE_3D:
            glTexStorage3D(target, dds.GetMipCount(), format.m_format,
                dds.GetWidth(), dds.GetHeight(), dds.GetDepth());
            break;
        case GL_TEXTURE_CUBE_MAP_ARRAY:
            glTexStorage3D(target, dds.GetMipCount(), format.m_format,
                dds.GetWidth(), dds.GetHeight(), dds.GetArraySize());
            break;
        default:
            glBindTexture(target, 0);
            return false;
        }
        dds.Flip();

        uint32_t numFaces = dds.IsCubemap() ? 6 : 1;
        for (uint32_t layer = 0; layer < dds.GetArraySize(); layer++) 
        {
            for (uint32_t face = 0; face < numFaces; face++) 
            {
                for (uint32_t level = 0; level < dds.GetMipCount(); level++) 
                {
                    GLenum target2 = dds.IsCubemap()
                        ? (GL_TEXTURE_CUBE_MAP_POSITIVE_X + face)
                        : target;
                    auto imageData = dds.GetImageData(level, layer * numFaces);
                    switch (target) 
                    {
                    case GL_TEXTURE_1D:
                        if (IsCompressed(format.m_format)) 
                        {
                            glCompressedTexSubImage1D(
                                target2, level, 0, imageData->m_width,
                                format.m_format, imageData->m_memSlicePitch,
                                imageData->m_mem);
                        }
                        else 
                        {
                            glTexSubImage1D(target2, level, 0,
                                imageData->m_width, format.m_format,
                                format.m_type, imageData->m_mem);
                        }
                        break;
                    case GL_TEXTURE_1D_ARRAY:
                    case GL_TEXTURE_2D:
                    case GL_TEXTURE_CUBE_MAP: 
                    {
                        auto w = imageData->m_width;
                        auto h = isArray ? layer : imageData->m_height;
                        if (IsCompressed(format.m_format)) 
                        {
                            glCompressedTexSubImage2D(
                                target2, level, 0, 0, w, h, format.m_format,
                                imageData->m_memSlicePitch, imageData->m_mem);
                        }
                        else 
                        {
                            glTexSubImage2D(target2, level, 0, 0, w, h,
                                format.m_format, format.m_type,
                                imageData->m_mem);
                        }
                        break;
                    }
                    default:
                        glBindTexture(target, 0);
                        return false;
                    }
                }
            }
        }

        glBindTexture(target, 0);
        return true;
    }

    bool ExtractTextureData(tinyddsloader::DDSFile& dds, tinyddsloader::DDSFile::ImageData& _DataExtract)
    {
        GLFormat format;
        if (!TranslateFormat(dds.GetFormat(), &format))
        {
            return false;
        }
        _DataExtract = *dds.GetImageData(0, 0);

        return true;
    }

    void TexLoader::SetTextureFlip(bool _Flip)
    {
        stbi_set_flip_vertically_on_load(_Flip);
    }

    // Load texture with .dds format using TinyDDSLoader
    //unsigned int TexLoader::LoadTexture(std::string _Path, std::string _Directory)
    //{
    //    std::string filename = _Directory + '/' + _Path;

    //    unsigned int textureID;
    //    glGenTextures(1, &textureID);

    //    tinyddsloader::DDSFile dds;
    //    auto ret = dds.Load(filename.c_str());
    //    if (tinyddsloader::Result::Success != ret) 
    //    {
    //        std::string err = "Texture failed to load at ";
    //       err += _Directory + '/' + _Path;
    //       Logger::Log({ "PP::TEXURE LOADER",LogEntry::ERROR, err });
    //    }
    //    else
    //    {
    //        if (!LoadGLTexture(textureID, dds))
    //        {
    //            std::string err = "LoadGLTexture failed to load at ";
    //            err += _Directory + '/' + _Path;
    //            Logger::Log({ "PP::TEXURE LOADER",LogEntry::ERROR, err });
    //        }
    //    }
    //    return textureID;
    //}
    
    // Load texture using STB
    unsigned int TexLoader::LoadTexture(std::string _Path, std::string _Directory)
	{
        std::string filename = _Directory + '/' + _Path;
        size_t found = _Path.find_last_of('.');
        std::string ext = _Path.substr(found + 1, _Path.length() - found - 1);

        unsigned int textureID;
        glGenTextures(1, &textureID);

        if (ext.compare("dds") == 0)
        {
            tinyddsloader::DDSFile dds;
            auto ret = dds.Load(filename.c_str());
            dds.Flip();
            if (tinyddsloader::Result::Success != ret)
            {
                std::string err = "Texture failed to load at ";
                err += _Directory + '/' + _Path;
                Logger::Log({ "PP::TEXURE LOADER",LogEntry::ERROR, err });
            }
            else
            {
                if (!LoadGLTexture(textureID, dds))
                {
                    std::string err = "LoadGLTexture failed to load at ";
                    err += _Directory + '/' + _Path;
                    Logger::Log({ "PP::TEXURE LOADER",LogEntry::ERROR, err });
                }
            }
        }
        else
        {
            int width, height, nrComponents;
            unsigned char* data = stbi_load(filename.c_str(), &width, &height, &nrComponents, 0);
            if (data)
            {
                GLenum format = 0;
                if (nrComponents == 1)
                    format = GL_RED;
                else if (nrComponents == 3)
                    format = GL_RGB;
                else if (nrComponents == 4)
                    format = GL_RGBA;

                glBindTexture(GL_TEXTURE_2D, textureID);
                glTexImage2D(GL_TEXTURE_2D, 0, format, width, height, 0, format, GL_UNSIGNED_BYTE, data);
                glGenerateMipmap(GL_TEXTURE_2D);

                glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
                glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);
                glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR_MIPMAP_LINEAR);
                glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);

                stbi_image_free(data);
            }
            else
            {
                std::string err = "Texture failed to load at ";
                err += _Directory + '/' + _Path;
                Logger::Log({ "PP::TEXURE LOADER",LogEntry::ERROR, err });
                stbi_image_free(data);
            }
        }

        return textureID;
	}

    unsigned int TexLoader::LoadTextureSRGB(std::string _Path, std::string _Directory, bool _Alpha)
    {
        const GLint format = _Alpha ? GL_COMPRESSED_SRGB_ALPHA_S3TC_DXT1_EXT : GL_COMPRESSED_SRGB_S3TC_DXT1_EXT;

        std::string filename = _Directory + '/' + _Path;

        tinyddsloader::DDSFile dds;
        dds.Load(filename.c_str());
        dds.Flip();
        tinyddsloader::DDSFile::ImageData imageData;
        ExtractTextureData(dds, imageData);
        unsigned int textureID;
        glGenTextures(1, &textureID);

        if (imageData.m_width != 0 && imageData.m_height != 0)
        {
            glBindTexture(GL_TEXTURE_2D, textureID);
            //glTexImage2D(GL_TEXTURE_2D, 0, format, imageData.m_width, imageData.m_height, 0, format, GL_UNSIGNED_BYTE, imageData.m_mem);

            glCompressedTexImage2D
            (
                GL_TEXTURE_2D,
                0,
                format,
                imageData.m_width,
                imageData.m_height,
                0,
                imageData.m_memSlicePitch,
                imageData.m_mem
            );

            glGenerateMipmap(GL_TEXTURE_2D);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR_MIPMAP_LINEAR);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
            glBindTexture(GL_TEXTURE_2D, 0);
        }
        else
        {
            std::string err = "Texture failed to load at ";
            err += _Directory + '/' + _Path;
            Logger::Log({ "PP::TEXURE LOADER",LogEntry::ERROR, err });
        }

        return textureID;
    }

    unsigned int TexLoader::LoadUncompressedTexture(std::string _Path, std::string _Directory, bool _Alpha)
    {
        const GLint format = _Alpha ? GL_RGBA : GL_RGB;

        std::string filename = _Directory + '/' + _Path;

        tinyddsloader::DDSFile dds;
        dds.Load(filename.c_str());
        dds.Flip();
        tinyddsloader::DDSFile::ImageData imageData;
        ExtractTextureData(dds, imageData);
        unsigned int textureID;
        glGenTextures(1, &textureID);

        if (imageData.m_width != 0 && imageData.m_height != 0)
        {
            glBindTexture(GL_TEXTURE_2D, textureID);
            glTexImage2D(GL_TEXTURE_2D, 0, format, imageData.m_width, imageData.m_height, 0, format, GL_UNSIGNED_BYTE, imageData.m_mem);
            glGenerateMipmap(GL_TEXTURE_2D);

            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR_MIPMAP_LINEAR);
            glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
            glBindTexture(GL_TEXTURE_2D, 0);
        }
        else
        {
            std::string err = "Texture failed to load at ";
            err += _Directory + '/' + _Path;
            Logger::Log({ "PP::TEXURE LOADER",LogEntry::ERROR, err });
        }

        return textureID;
    }

    unsigned int TexLoader::LoadCubemap(std::vector<std::string> _Paths, std::string _Directory)
    {
        unsigned int textureID;
        glGenTextures(1, &textureID);
        glBindTexture(GL_TEXTURE_CUBE_MAP, textureID);

        for (unsigned int i = 0; i < _Paths.size(); i++)
        {
            std::string filename = _Directory + '/' + _Paths[i];

            //unsigned char* data = stbi_load(filename.c_str(), &width, &height, &nrChannels, 0);

            tinyddsloader::DDSFile dds;
            dds.Load(filename.c_str());
            //dds.Flip();
            tinyddsloader::DDSFile::ImageData imageData;
            ExtractTextureData(dds, imageData);

            if (imageData.m_width !=0 && imageData.m_height != 0)
            {
                glCompressedTexImage2D
                (
                    GL_TEXTURE_CUBE_MAP_POSITIVE_X + i,
                    0,
                    GL_COMPRESSED_SRGB_S3TC_DXT1_EXT,
                    imageData.m_width,
                    imageData.m_height,
                    0,
                    imageData.m_memSlicePitch,
                    imageData.m_mem
                );
                //stbi_image_free(data);
            }
            else
            {
                std::string err = "Texture failed to load at ";
                err += _Directory + '/' + _Paths[i];
                Logger::Log({ "PP::TEXURE LOADER",LogEntry::ERROR, err });
                //stbi_image_free(data);
            }
        }

        glTexParameteri(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
        glTexParameteri(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
        glTexParameteri(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE);
        glTexParameteri(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE);
        glTexParameteri(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_WRAP_R, GL_CLAMP_TO_EDGE);
        glBindTexture(GL_TEXTURE_2D, 0);

        return textureID;
    }

    bool TexLoader::LoadHeightMap(std::string _Path, std::string _Directory, std::vector<unsigned char>& _HeightMap, size_t& _Dim)
    {
        std::string filename = _Directory + '/' + _Path;
        std::ifstream fileStream(filename, std::ios::binary);
        if (!fileStream.is_open())
        {
            std::string err = "Heightmap failed to load at ";
            err += filename;
            Logger::Log({ "PP::TEXURE LOADER",LogEntry::ERROR, err });
            return false;
        }

        fileStream.seekg(0, std::ios::end);
        std::streampos fsize = (unsigned)fileStream.tellg();
        fileStream.seekg(0, std::ios::beg);
        _HeightMap.resize((unsigned)fsize);
        _Dim = static_cast<size_t>(sqrt(_HeightMap.size()));
        fileStream.read((char*)&_HeightMap[0], fsize);

        fileStream.close();

        return true;
    }
    float TexLoader::GetHeight(int _X, int _Z, size_t _Dim, const std::vector<unsigned char>& _HeightMap)
    {
        if (_X < 0 || _X >= _Dim || _Z < 0 || _Z >= _Dim)
        {
            return 0.0f;
        }
        else
        {
            return _HeightMap[_Z * _Dim + _X];
        }
    }
}
