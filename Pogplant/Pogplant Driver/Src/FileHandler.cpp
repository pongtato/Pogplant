/*****************************************************************************/
/*!
\file	FileHandler.cpp
\author Clarence Chye Min Liang
\par	email: chye.m\@digipen.edu
\details
    Singleton FileHandler class used for file tracking and updates

\copyright	Copyright (c) 2021 DigiPen Institute of Technology. Reproduction
            or disclosure of this file or its contents without the prior
            written consent of DigiPen Institute of Technology is prohibited.
*/
/*****************************************************************************/

#include "FileHandler.h"
#include "AssetCompiler.h"
#include "Logger.h"

namespace PPF
{
    std::unique_ptr<FileHandler> FileHandler::m_instance = nullptr;
    std::once_flag FileHandler::m_onceFlag;;
    std::filesystem::path FileHandler::m_defaultpath;
    std::thread FileHandler::m_thread;

    FileHandler& FileHandler::GetInstance()
    {
        std::call_once(m_onceFlag, [] {
            m_instance.reset(new FileHandler);
            m_defaultpath = std::filesystem::current_path();
            m_thread = std::thread(&FileHandler::Start, m_instance.get());
            });

        return *m_instance.get();
    }

    void FileHandler::AddNewWatchPath(const std::string& path)
    {
        std::unordered_map<std::string, ftt> files;
        for (auto& file : std::filesystem::recursive_directory_iterator(path))
        {
            if (!file.is_directory())
            {
                files[file.path().string()] = std::filesystem::last_write_time(file);
            }
        }
        m_path[path] = files;
    }

    void FileHandler::Start()
    {
        while (m_running)
        {
            // Wait for "delay" milliseconds
            std::this_thread::sleep_for(m_delay);
            // Windows Dialog fix
            if (std::filesystem::current_path() != m_defaultpath)
            {
                continue;
            }

            // Iterator to the MAP (Path)
            auto it = m_path.begin();
            while (it != m_path.end())
            {
                // Iterator to the MAP (Files)
                auto it1 = it->second.begin();
                while (it1 != it->second.end())
                {
                    // Implement whatever logic for file erasure.
                    if (!std::filesystem::exists(it1->first))
                    {
                        //std::cout << "[PP::FILEHANDLER] " << it1->first << " ERASED" << std::endl;
                        PP::Logger::Log(PP::LogEntry{"PP::FILEHANDLER", PP::LogEntry::TYPE::SUCCESS, it->first + " erased."});
                        it1 = it->second.erase(it1);
                    }
                    else
                    {
                        it1++;
                    }
                }

                // Check the files in the directory if its changed or modified
                for (auto& file : std::filesystem::recursive_directory_iterator(it->first))
                {
                    auto current_file_last_write_time = std::filesystem::last_write_time(file);
                    if (file.is_directory())
                    {
                        //std::cout << "File is a directory, skipping." << std::endl;
                        continue;
                    }

                    std::string filePath = file.path().string();
                    std::string extension = file.path().extension().string();
                    std::string key = file.path().stem().string();
                    // File creation
                    if (!it->second.contains(filePath))
                    {
                        it->second[filePath] = current_file_last_write_time;
                        // Implement whatever logic for file creation
                        //std::cout << "[PP::FILEHANDLER] " << filePath << " CREATED" << std::endl;
                        PP::Logger::Log(PP::LogEntry{ "PP::FILEHANDLER", PP::LogEntry::TYPE::SUCCESS, filePath + " created."});

                        // File type is a compiled model
                        if (extension.compare(".kek") == 0)
                        {
                            // KEY will be the filename with extension, example: cube.kek
                            if (!PP::ModelResource::m_ModelPool.contains(key))
                            {
                                m_modelNew.push({ key, filePath });
                            }
                        }
                        // File type is an uncompiled model
                        else if (extension.compare(".fbx") == 0 || extension.compare(".obj") == 0)
                        {
                            if (!PP::ModelResource::m_ModelPool.contains(key))
                            {
                                // Compile the new model
                                PPC::AssetCompiler& ac = ac.GetInstance();
                                ac.RunExecutable("Pogplant Compiler.exe", filePath);
                                ac.WaitForSingleProcess(key);
                            }
                        }
                    }
                    // File modification
                    else
                    {
                        if (it->second[filePath] != current_file_last_write_time)
                        {
                            it->second[filePath] = current_file_last_write_time;
                            // Implement whatever logic for file modification
                            //std::cout << "[PP::FILEHANDLER] " << filePath << " MODIFIED" << std::endl;
                            PP::Logger::Log(PP::LogEntry{ "PP::FILEHANDLER", PP::LogEntry::TYPE::SUCCESS, filePath + " modified." });

                            // File type is a compiled model
                            if (extension.compare(".kek") == 0)
                            {
                                // KEY will be the filename with extension, example: cube.kek
                                if (PP::ModelResource::m_ModelPool.contains(key))
                                {
                                    m_modelUpdate.push({ key, filePath });
                                }
                            }
                            // File type is an uncompiled model
                            else if (extension.compare(".fbx") == 0 || extension.compare(".obj") == 0)
                            {
                                // Compile the new model
                                PPC::AssetCompiler& ac = ac.GetInstance();
                                ac.RunExecutable("Pogplant Compiler.exe", filePath);
                                ac.WaitForSingleProcess(key);
                            }
                        }
                    }
                }

                it++;
            }
        }
    }

    void FileHandler::Stop()
    {
        m_running = false;
        if (m_thread.joinable())
        {
            m_thread.join();
        }
    }

    void FileHandler::UpdateModels()
    {
        while (!m_modelNew.empty())
        {
            auto& modelNew = m_modelNew.top();
            PP::ModelResource::LoadModel(PP::ModelResource::m_ModelPool, modelNew.m_key, modelNew.m_filepath);
            m_modelNew.pop();
        }

        while (!m_modelUpdate.empty())
        {
            auto& modelUpdate = m_modelUpdate.top();
            PP::ModelResource::UpdateModel(PP::ModelResource::m_ModelPool, modelUpdate.m_key, modelUpdate.m_filepath);
            m_modelUpdate.pop();
        }
    }
}