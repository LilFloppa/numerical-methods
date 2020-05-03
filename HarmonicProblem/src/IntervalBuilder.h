#pragma once

#include "stdafx.h"
#include "FEMInfo.h"

class IntervalBuilder
{
public:
	IntervalBuilder(std::vector<RawInterval>& intervals)
	{
		for (auto i : intervals)
		{

			double begin = i.begin;
			double end = i.end;
			int n = i.n;
			double q = i.q;

			double h = (q == 1.0) ? ((end - begin) / n) : ((end - begin) * (1.0 - q) / (1.0 - pow(q, n)));

			Interval parsed;
			parsed.begin = begin;
			parsed.end = begin + h;
			parsed.n[0] = nodeCount++;
			parsed.n[1] = nodeCount;
			parsedIntervals.push_back(parsed);

			for (int i = 1; i < n; i++)
			{
				h *= q;

				Interval parsed;
				parsed.begin = parsedIntervals.back().end;
				parsed.end = parsed.begin + h;
				parsed.n[0] = nodeCount++;
				parsed.n[1] = nodeCount;

				parsedIntervals.push_back(parsed);
			}
		}

		nodeCount++;
	}

	IIterator Begin() { return parsedIntervals.begin(); }
	IIterator End() { return parsedIntervals.end(); }

	int NodeCount() { return nodeCount; }

private:
	std::vector<Interval> parsedIntervals;
	int nodeCount = 0;
};