#include "FileHandler.h"

std::unique_ptr<FileHandler> FileHandler::m_instance = nullptr;
std::once_flag FileHandler::m_onceFlag;

FileHandler& FileHandler::GetInstance()
{
    std::call_once(m_onceFlag, [] {
        m_instance.reset(new FileHandler);
        });

    return *m_instance.get();
}

void FileHandler::AddNewWatchPath(std::string path)
{
    std::unordered_map<std::string, ftt> files;
    for (auto& file : std::filesystem::recursive_directory_iterator(path))
    {
        files[file.path().string()] = std::filesystem::last_write_time(file);
    }

    m_path[path] = files;
    // Each new folder is a new thread, might change in the future
    m_threads[path] = std::thread{&FileHandler::Start, this};
}

void FileHandler::Start()
{
	while(m_running)
    {
        // Wait for "delay" milliseconds
        std::this_thread::sleep_for(m_delay);
        
        // Iterator to the MAP (Path)
        auto it = m_path.begin();
        while (it != m_path.end()) 
        {
            // Iterator to the VECTOR (File)
            auto it1 = it->second.begin();
            while (it1 != it->second.end())
            {
                if (!std::filesystem::exists(it1->first))
                {
                    std::cout << it1->first << " ERASED" << std::endl;
                    it1 = it->second.erase(it1);
                }
                else
                {
                    it1++;
                }
            }
            it++;
        }
        
        // Iterator to the MAP (Path)
        auto it2 = m_path.begin();
        while (it2 != m_path.end())
        {
            // Check if a file was created or modified
            for (auto& file : std::filesystem::recursive_directory_iterator(it2->first))
            {
                auto current_file_last_write_time = std::filesystem::last_write_time(file);
                // File creation
                if (!it2->second.contains(file.path().string()))
                {
                    it2->second[file.path().string()] = current_file_last_write_time;
                    // CREATED
                    std::cout << file.path().string() << " CREATED" << std::endl;
                    std::string key = PP::ModelResource::GetFileName(file.path().string());
                    if (!PP::ModelResource::m_ModelPool.contains(key))
                    {
                        PP::ModelResource::LoadModel(key, file.path().string());
                        m_key = key;
                        m_update = true;
                    }
                }
                // File modification
                else
                {
                    if (it2->second[file.path().string()] != current_file_last_write_time)
                    {
                        it2->second[file.path().string()] = current_file_last_write_time;
                        // MODIFIED
                        std::cout << file.path().string() << " MODIFIED" << std::endl;

                        std::string key = PP::ModelResource::GetFileName(file.path().string());
                        if (PP::ModelResource::m_ModelPool.contains(key))
                        {
                            PP::ModelResource::LoadModel(key, file.path().string());
                            m_key = key;
                            m_update = true;
                        }
                    }
                }
            }

            ++it2;
        }
    }
}

void FileHandler::Stop()
{
    m_running = false;
    auto it = m_threads.begin();
    while (it != m_threads.end())
    {
        if (it->second.joinable())
        {
            it->second.join();
        }
        ++it;
    }
}

bool FileHandler::GetUpdate()
{
    return m_update;
}

void FileHandler::SetUpdate(bool update)
{
    m_update = update;
}

std::string FileHandler::GetKey()
{
    return m_key;
}

void FileHandler::SetKey(std::string key)
{
    m_key = key;
}