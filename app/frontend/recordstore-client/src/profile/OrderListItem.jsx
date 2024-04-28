import {
  Badge,
  Box,
  Flex,
  Heading,
  Text,
  Collapse,
  useDisclosure,
  UnorderedList,
  Select,
  ListItem,
  useToast,
} from '@chakra-ui/react';
import { Link } from 'react-router-dom';

import { useState } from 'react';

const statusMap = {
  Pending: { color: 'gray', text: 'Очікується' },
  Paid: { color: 'blue', text: 'Оплачено' },
  Processing: { color: 'blue', text: 'Обробляється' },
  Shipped: { color: 'blue', text: 'Відправлено' },
  Delivered: { color: 'green', text: 'Доставлено' },
  Canceled: { color: 'red', text: 'Скасовано' },
};

import formatDate from '../utils/formatDate';
import formatCurrency from '../utils/formatCurrency';

const OrderListItem = ({
  order,
  manage = false,
  statusOptions,
  updateStatus,
  setOrder,
}) => {
  const { isOpen, onToggle } = useDisclosure();
  const [thisOrder, setThisOrder] = useState(order);

  const toast = useToast();

  const handleStatusChange = async (event) => {
    const newStatus = {
      Name: event.target.value,
    };
    const response = await updateStatus(order.id, newStatus);

    console.log(response);
    if (response.success !== true) {
      toast({
        title: 'Помилка',
        description:
          'Не вдалося оновити статус замовлення. ' + response.message,
        status: 'error',
        duration: 5000,
        isClosable: true,
      });
    }
  };

  return (
    <Flex
      w='full'
      boxShadow='md'
      p={6}
      mb={4}
      key={order.id}
      borderRadius='xl'
      bg='white'
      _hover={{ boxShadow: 'xl', transform: 'translateY(-2px)' }}
      transition='all 0.3s ease-in-out'
      cursor='pointer'
      onClick={onToggle}
    >
      <Box w='full'>
        <Flex justify='space-between' align='center' mb={4}>
          <Heading size='md' color='gray.700'>
            Замовлення №{order.id}
          </Heading>
          {manage ? (
            <Select
              value={order.status}
              onChange={handleStatusChange}
              maxW='200px'
              borderRadius='full'
              fontSize='sm'
              textTransform='uppercase'
              letterSpacing='wider'
              onClick={(e) => e.stopPropagation()}
            >
              {statusOptions.map((status) => (
                <option key={status} value={status}>
                  {statusMap[status]?.text || status}
                </option>
              ))}
            </Select>
          ) : (
            <Badge
              colorScheme={statusMap[order.status].color || 'gray'}
              px={3}
              py={1}
              borderRadius='full'
              fontSize='sm'
              textTransform='uppercase'
              letterSpacing='wider'
            >
              {statusMap[order.status].text || order.status}
            </Badge>
          )}
        </Flex>
        <Text color='gray.500' mb={2}>
          {formatDate(order.createdAt)}
        </Text>
        <Collapse in={isOpen} animateOpacity unmountOnExit>
          <Heading size='sm' mt={4} mb={2} color='gray.600'>
            Товари
          </Heading>
          <Box>
            <UnorderedList spacing={4}>
              {order.items.map((item) => (
                <ListItem
                  key={item.id}
                  display='flex'
                  w='40rem'
                  justifyContent='space-between'
                  alignItems='center'
                  _hover={{ bg: 'gray.100', borderRadius: 'md' }}
                  px={3}
                  py={2}
                  transition='background-color 0.3s ease-in-out'
                >
                  <Text
                    as={Link}
                    to={`/products/${item.product.id}`}
                    className='hover:text-blue-600 text-blue-600 font-semibold'
                  >
                    {item.product.title}
                  </Text>
                  <Text color='gray.500'>
                    {item.quantity} x {formatCurrency(item.price)}
                  </Text>
                </ListItem>
              ))}
            </UnorderedList>
            <Flex justify='space-between' align='center' mt='6'>
              <Text color='gray.600' fontSize='lg' fontWeight='semibold'>
                Загалом:
              </Text>
              <Text color='gray.800' fontSize='xl' fontWeight='bold'>
                {formatCurrency(order.total)}
              </Text>
            </Flex>
          </Box>
        </Collapse>
      </Box>
    </Flex>
  );
};

export default OrderListItem;
