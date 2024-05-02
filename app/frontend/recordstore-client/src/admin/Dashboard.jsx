import {
  FormLabel,
  FormControl,
  Input,
  Button,
  Flex,
  Heading,
  Link,
  Icon,
  Text,
  Box,
  IconButton,
} from '@chakra-ui/react';

import { Select, SelectItem, BarChart } from '@tremor/react';

import { HiHome, HiCube, HiMiniUserGroup } from 'react-icons/hi2';

import { HiSearch } from 'react-icons/hi';

import { useStats } from '../hooks/useStats';

import { useState, useEffect } from 'react';
import LineChart from './LineChart';

import formatCurrency from '../utils/formatCurrency';
import Users from './Users';

const Dashboard = () => {
  const [activePage, setActivePage] = useState('Огляд');
  const {
    financialStats,
    financialChartStats,
    orderChartStats,
    loading,
    error,
    period,
    setPeriod,
    fetchOrderChartStatsById,
    fetchQuantityChartStats,
    fetchAverageOrderValueChartStats,
    fetchFinancialStats,
    fetchFinancialChartStats,
    fetchOrderChartStats,
    quantityChartStats,
    averageOrderValueChartStats,
    ordersPerRegion,
  } = useStats();

  const [orderId, setOrderId] = useState(0);

  const Links = [
    {
      name: 'Огляд',
      icon: HiHome,
      path: '/admin',
    },
    {
      name: 'Замовлення',
      icon: HiCube,
      path: '/admin/orders',
    },
    {
      name: 'Користувачі',
      icon: HiMiniUserGroup,
      path: '/admin/users',
    },
  ];

  const handleFetchOrderStats = (id) => {
    console.log(id);
    if (!id) {
      return fetchOrderChartStats(period);
    }
    fetchOrderChartStatsById(id, period);
    fetchQuantityChartStats(id, period);
  };

  return (
    <Box bg='gray.100' flexGrow={1} display={'flex'} flexDir={'row'}>
      {/* col1 */}
      <Flex
        w='15%'
        flexDir='column'
        alignItems='center'
        backgroundColor='#020202'
        color='#fff'
      >
        <Flex
          flexDir='column'
          justifyContent='space-between'
          position={'fixed'}
        >
          <Flex
            flexDir='column'
            alignItems='flex-start'
            justifyContent='center'
            mt='50'
          >
            {Links.map((link) => (
              <Flex
                className={
                  'sidebar-items' + (activePage === link.name ? ' active' : '')
                }
                key={link.name}
                onClick={() => setActivePage(link.name)}
              >
                <Link>
                  <Icon as={link.icon} fontSize='2xl'></Icon>
                </Link>
                <Link _hover={{ textDecoration: 'none' }}>
                  <Text>{link.name}</Text>
                </Link>
              </Flex>
            ))}
          </Flex>
        </Flex>
      </Flex>
      {/* col2 */}
      <Flex p='10' flexDir='column' flexGrow={1} px='15rem'>
        <Heading>Інформаційна панель</Heading>
        {activePage === 'Огляд' && (
          <>
            <Heading as='h3' size='lg' mt='5' mb='4'>
              Огляд
            </Heading>
            <Box>
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
                  <LineChart
                    data={financialChartStats}
                    valueFormatter={formatCurrency}
                  />
                </>
              )}
            </Box>
          </>
        )}
        {activePage === 'Замовлення' && (
          <>
            <Heading as='h3' size='lg' mt='5' mb='4'>
              Замовлення
            </Heading>
            <Box>
              <Flex
                flexDir='row'
                justifyContent='space-between'
                alignItems='center'
                w='100%'
              >
                <div>
                  <h3 className='text-tremor-default text-tremor-content mt-4'>
                    Замовлення
                  </h3>
                  <p className='text-tremor-metric text-tremor-content-strong font-semibold'>
                    {Intl.NumberFormat('uk-UA').format(
                      financialStats.totalOrders
                    )}
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
              <FormControl>
                <FormLabel htmlFor='order-id' mt='4'>
                  <Text fontSize='md' as='b'>
                    ID продукту
                  </Text>
                </FormLabel>
                <Flex w='100%' gap='4'>
                  <Input
                    id='order-id'
                    type='number'
                    value={orderId}
                    onChange={(e) => {
                      if (e.target.value === '') {
                        setOrderId(0);
                      }

                      const value = parseInt(e.target.value);
                      if (!isNaN(value)) {
                        setOrderId(value);
                      }
                    }}
                  />
                  <IconButton
                    aria-label='Пошук'
                    icon={<HiSearch />}
                    onClick={() => handleFetchOrderStats(orderId)}
                    colorScheme='green'
                  />
                </Flex>
              </FormControl>
              <Heading as='h4' size='md' mt='6'>
                Кількість замовлень за обраний період
              </Heading>
              <LineChart data={orderChartStats} />
              {quantityChartStats && quantityChartStats.length > 0 ? (
                <>
                  <Heading as='h4' size='md' mt='6'>
                    Кількість проданих одиниць за обраний період
                  </Heading>
                  <LineChart data={quantityChartStats} />
                </>
              ) : (
                <>
                  <Heading as='h4' size='md' mt='10'>
                    Середня вартість замовлень за обраний період
                  </Heading>
                  <LineChart
                    data={averageOrderValueChartStats}
                    valueFormatter={formatCurrency}
                  />
                  <Heading as='h4' size='md' mt='10'>
                    Замовлення за областями
                  </Heading>
                  <BarChart
                    data={ordersPerRegion}
                    index='region'
                    categories={['ordersCount']}
                  />
                </>
              )}
            </Box>
          </>
        )}
        {activePage === 'Користувачі' && <Users />}
      </Flex>
    </Box>
  );
};

export default Dashboard;
