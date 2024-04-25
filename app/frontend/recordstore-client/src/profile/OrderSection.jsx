import {
  Box,
  Text,
  Heading,
  Flex,
  Badge,
  Select,
  Button,
  Divider,
  HStack,
} from '@chakra-ui/react';
import OrderListItem from './OrderListItem';

import { useState, useEffect } from 'react';

// order by: orderDate, totalPrice
// page

const orderByOptions = [
  { value: 'orderDate', label: 'Дата замовлення' },
  { value: 'totalPrice', label: 'Сума' },
];

const orderDirectionOptions = [
  { value: 'asc', label: 'За зростанням' },
  { value: 'desc', label: 'За спаданням' },
];

const OrderSection = ({ orders, fetchOrders }) => {
  const [params, setParams] = useState({
    orderBy: orderByOptions[0].value,
    orderDirection: orderDirectionOptions[0].value,
    page: 1,
  });

  const handleOrderByChange = (e) => {
    setParams({ ...params, orderBy: e.target.value });
  };

  const handleOrderDirectionChange = (e) => {
    setParams({ ...params, orderDirection: e.target.value });
  };

  const handlePageChange = (page) => {
    setParams({ ...params, page });
  };

  const handleApplyFilters = () => {
    const updatedParams = { ...params, page: 1 };
    setParams(updatedParams);
    fetchOrders(updatedParams);
  };

  useEffect(() => {
    fetchOrders(params);
  }, [params.page]);

  return (
    <Box>
      <Flex justify='space-between' align='center' mb={4}>
        <Heading size='md'>Замовлення</Heading>
        <Flex w='full' justify='flex-end'>
          <Select
            mr={2}
            value={params.orderBy}
            onChange={handleOrderByChange}
            placeholder='Сортувати за'
            w='auto'
          >
            {orderByOptions.map((option) => (
              <option key={option.value} value={option.value}>
                {option.label}
              </option>
            ))}
          </Select>
          <Select
            mr={2}
            value={params.orderDirection}
            onChange={handleOrderDirectionChange}
            placeholder='Порядок'
            w='auto'
          >
            {orderDirectionOptions.map((option) => (
              <option key={option.value} value={option.value}>
                {option.label}
              </option>
            ))}
          </Select>
          <Button colorScheme='blue' onClick={handleApplyFilters} minW='auto'>
            Застосувати
          </Button>
        </Flex>
      </Flex>
      <Divider mb={4} />
      <Box>
        {orders !== null && orders.results.length > 0 ? (
          <>
            {orders.results.map((order) => (
              <OrderListItem key={order.id} order={order} />
            ))}
            <HStack mt={4} justify='center'>
              <Button
                isDisabled={params.page === 1}
                onClick={() => handlePageChange(params.page - 1)}
                size='sm'
              >
                Попередня
              </Button>
              <Text>{params.page}</Text>
              <Button
                isDisabled={params.page === orders.pageCount}
                onClick={() => handlePageChange(params.page + 1)}
                size='sm'
              >
                Наступна
              </Button>
            </HStack>
          </>
        ) : (
          <Text>No orders</Text>
        )}
      </Box>
    </Box>
  );
};

export default OrderSection;
