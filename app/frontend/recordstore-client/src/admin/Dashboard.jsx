import { Flex, Heading, Link, Icon, Text, Box } from '@chakra-ui/react';
import { HiHome, HiCube, HiMiniUserGroup } from 'react-icons/hi2';
import { useStats } from './useStats';

import { useState, useEffect } from 'react';

import {
  AreaChart,
  MultiSelect,
  MultiSelectItem,
  SearchSelect,
  SearchSelectItem,
  Select,
  SelectItem,
} from '@tremor/react';

import formatCurrency from '../utils/formatCurrency';

const Dashboard = () => {
  const [activePage, setActivePage] = useState('overview');
  const {
    financialStats,
    financialChartStats,
    loading,
    error,
    period,
    setPeriod,
  } = useStats();

  return (
    <Flex h='100vh' flexDir='row' overflow='hidden' maxW='2000px'>
      {/* col1 */}
      <Flex
        w='15%'
        flexDir='column'
        alignItems='center'
        backgroundColor='#020202'
        color='#fff'
      >
        <Flex flexDir='column' justifyContent='space-between'>
          <Flex
            flexDir='column'
            alignItems='flex-start'
            justifyContent='center'
            mt='50'
          >
            <Flex className='sidebar-items'>
              <Link>
                <Icon as={HiHome} fontSize='2xl'></Icon>
              </Link>
              <Link _hover={{ textDecoration: 'none' }}>
                <Text>Огляд</Text>
              </Link>
            </Flex>
            <Flex className='sidebar-items'>
              <Link>
                <Icon as={HiCube} fontSize='2xl'></Icon>
              </Link>
              <Link _hover={{ textDecoration: 'none' }}>
                <Text>Замовлення</Text>
              </Link>
            </Flex>
            <Flex className='sidebar-items'>
              <Link>
                <Icon as={HiMiniUserGroup} fontSize='2xl'></Icon>
              </Link>
              <Link _hover={{ textDecoration: 'none' }}>
                <Text>Користувачі</Text>
              </Link>
            </Flex>
          </Flex>
        </Flex>
      </Flex>
      {/* col2 */}
      <Flex p='10' flexDir='column' w='85%'>
        <Heading>Інформаційна панель</Heading>
        {activePage === 'overview' && (
          <>
            <Heading as='h3' size='lg' mt='5'>
              Огляд
            </Heading>
            <Box w='100%'>
              {loading && <p>Loading...</p>}
              {error && <p>Error: {error.message}</p>}
              <Flex w='100%' mt='4' gap='4'>
                <Box flexGrow='1' className='financial-stats'>
                  <Heading as='h4' size='md'>
                    Загальна кількість замовлень
                  </Heading>
                  {financialStats.totalOrders}
                </Box>
                <Box flexGrow='1' className='financial-stats'>
                  <Heading as='h4' size='md'>
                    Загальний дохід
                  </Heading>
                  {formatCurrency(financialStats.totalIncome)}
                </Box>
                <Box flexGrow='1' className='financial-stats'>
                  <Heading as='h4' size='md'>
                    Загальні витрати
                  </Heading>
                  {formatCurrency(financialStats.totalExpenses)}
                </Box>
                <Box flexGrow='1' className='financial-stats'>
                  <Heading as='h4' size='md'>
                    Чистий прибуток
                  </Heading>
                  {formatCurrency(financialStats.netIncome)}
                </Box>
              </Flex>
              {financialChartStats.length > 0 && (
                <>
                  <Flex
                    flexDir='row'
                    justifyContent='space-between'
                    alignItems='center'
                    w='100%'
                  >
                    <div>
                      <h3 className='text-tremor-default text-tremor-content mt-4'>
                        Фінансова звітність
                      </h3>
                      <p className='text-tremor-metric text-tremor-content-strong font-semibold'>
                        {formatCurrency(financialStats.netIncome)}
                      </p>
                    </div>
                    <div className='w-100'>
                      <Select value={period} onChange={setPeriod}>
                        <SelectItem value='week'>Тиждень</SelectItem>
                        <SelectItem value='month'>Місяць</SelectItem>
                        <SelectItem value='year'>Рік</SelectItem>
                      </Select>
                    </div>
                  </Flex>
                  <AreaChart
                    className='mt-4 h-72'
                    data={financialChartStats}
                    index='date'
                    yAxisWidth={90}
                    categories={['totalIncome', 'totalExpenses', 'netIncome']}
                    colors={['indigo', 'rose', 'green']}
                    seriesField='type'
                    valueFormatter={formatCurrency}
                    height={300}
                    padding={40}
                  />
                </>
              )}
            </Box>
          </>
        )}
      </Flex>
      {/* col3 */}
      <Flex></Flex>
    </Flex>
  );
};

export default Dashboard;
