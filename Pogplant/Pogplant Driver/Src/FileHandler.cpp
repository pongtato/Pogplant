#include "FileHandler.h"
#include "AssetCompiler.h"

std::unique_ptr<FileHandler> FileHandler::m_instance = nullptr;
std::once_flag FileHandler::m_onceFlag;;
std::filesystem::path FileHandler::m_defaultpath;
std::thread FileHandler::m_thread;

FileHandler& FileHandler::GetInstance()
{
    std::call_once(m_onceFlag, [] {
        m_instance.reset(new FileHandler);
        m_defaultpath = std::filesystem::current_path();
        m_thread = std::thread( &FileHandler::Start, m_instance.get());
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
	while(m_running)
    {
        // Wait for "delay" milliseconds
        std::this_thread::sleep_for(m_delay);
        //Windows Dialog fix
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
                if (!std::filesystem::exists(it1->first))
                {
                    std::cout << "[PP::FILEHANDLER] " << it1->first << " ERASED" << std::endl;
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
                    // CREATED
                    std::cout << "[PP::FILEHANDLER] " << filePath << " CREATED" << std::endl;
                    
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
                    else if (extension.compare(".fbx") == 0 || extension.compare(".obj"))
                    {
                        if (!PP::ModelResource::m_ModelPool.contains(key))
                        {
                            // Compile the new model
                            AssetCompiler& ac = ac.GetInstance();
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
                        // MODIFIED
                        std::cout << "[PP::FILEHANDLER] " << filePath << " MODIFIED" << std::endl;

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
                        else if (extension.compare(".fbx") == 0 || extension.compare(".obj"))
                        {
                            // Compile the new model
                            AssetCompiler& ac = ac.GetInstance();
                            ac.RunExecutable("Pogplant Compiler.exe", filePath);
                            ac.WaitForSingleProcess(key);
                        }
                    }
                }
            }

            it++;
        }

        //// Iterator to the MAP (Path)
        //auto it = m_path.begin();
        //while (it != m_path.end()) 
        //{
        //    // Iterator to the MAP (Files)
        //    auto it1 = it->second.begin();
        //    while (it1 != it->second.end())
        //    {
        //        if (!std::filesystem::exists(it1->first))
        //        {
        //            std::cout << it1->first << " ERASED" << std::endl;
        //            it1 = it->second.erase(it1);
        //        }
        //        else
        //        {
        //            it1++;
        //        }
        //    }
        //    it++;
        //}
        //
        //// Iterator to the MAP (Path)
        //auto it2 = m_path.begin();
        //while (it2 != m_path.end())
        //{
        //    // Check the files in the directory if its changed or modified
        //    for (auto& file : std::filesystem::recursive_directory_iterator(it2->first))
        //    {
        //        auto current_file_last_write_time = std::filesystem::last_write_time(file);
        //        // File creation
        //        if (!it2->second.contains(file.path().string()))
        //        {
        //            it2->second[file.path().string()] = current_file_last_write_time;
        //            // CREATED
        //            std::cout << file.path().string() << " CREATED" << std::endl;

        //            // KEY will be the filename, example: Copy.kek, key = "Copy"
        //            std::string key = GetFileName(file.path().string());
        //            if (!PP::ModelResource::m_ModelPool.contains(key))
        //            {
        //                m_modelNew.push({ key, file.path().string() });
        //            }
        //        }
        //        // File modification
        //        else
        //        {
        //            if (it2->second[file.path().string()] != current_file_last_write_time)
        //            {
        //                it2->second[file.path().string()] = current_file_last_write_time;
        //                // MODIFIED
        //                std::cout << file.path().string() << " MODIFIED" << std::endl;

        //                // KEY will be the filename, example: Copy.kek, key = "Copy"
        //                std::string key = GetFileName(file.path().string());
        //                if (PP::ModelResource::m_ModelPool.contains(key))
        //                {
        //                    m_modelUpdate.push({ key, file.path().string() });
        //                }
        //            }
        //        }
        //    }

        //    ++it2;
        //}
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