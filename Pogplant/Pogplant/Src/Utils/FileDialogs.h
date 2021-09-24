/******************************************************************************
* \file FileDialogs.h
* \brief WindowsFile dialogue that can be used to retrieve files
*
* \author Wee Ding Wei, 100% Code Contribution
*
*		copyright Copyright (c) 2020 DigiPen Institute of Technology.
    Reproduction or disclosure of this file or its contents without the prior
    written consent of DigiPen Institute of Technology is prohibited.
******************************************************************************/
#ifndef FILEDIALOGS_H
#define FILEDIALOGS_H

#include <string>

namespace Pogplant 
{
  class FileDialogs
  {
  public:
    static std::string OpenFile(const char* filter);
    static std::string SaveFile(const char* filter);
  };
}


#endif