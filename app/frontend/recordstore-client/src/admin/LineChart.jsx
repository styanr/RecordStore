import React from 'react';
import { AreaChart } from '@tremor/react';
import { Box } from '@chakra-ui/react';

const colors = ['indigo', 'rose', 'green', 'blue', 'yellow', 'purple'];

const LineChart = ({ data, valueFormatter }) => {
  if (!data || !data.length) {
    return <div className='text-center text-gray-500'>Дані відсутні</div>;
  }

  const categories = Object.keys(data[0]).filter((key) => key !== 'date');
  const categoriesColors = colors.slice(0, categories.length);

  return (
    <Box>
      <AreaChart
        className='mt-4 h-72'
        data={data}
        index='date'
        yAxisWidth={90}
        categories={categories}
        colors={categoriesColors}
        valueFormatter={valueFormatter || ((value) => value)}
        height={300}
        padding={40}
      />
    </Box>
  );
};

export default LineChart;
