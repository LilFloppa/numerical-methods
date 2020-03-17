#include <iostream>
#include <string>

#define RESET   "\033[0m"
#define RED     "\033[31m"
#define GREEN   "\033[32m"
#define YELLOW  "\033[33m"

namespace logging
{
	void success(std::string info)
	{
		std::cout <<  GREEN << info << RESET << std::endl;
	}

	void info(std::string info)
	{
		std::cout << info << std::endl;
	}

	void warning(std::string info)
	{
		std::cout << YELLOW << info << RESET << std::endl;
	}

	void error(std::string info)
	{
		std::cout << RED << info << RESET << std::endl;
	}
}