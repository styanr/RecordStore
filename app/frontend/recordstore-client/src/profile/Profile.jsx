import React, { useEffect, useState } from 'react';
import { Flex, Box, Avatar, Text, Button, useToast } from '@chakra-ui/react';
import CustomBadge from './CustomBadge';

import useAuth from '../hooks/useAuth';
import useAddress from '../hooks/useAddress';
import useOrders from '../hooks/useOrders';

import stringToColor from '../utils/stringToColor';
import AddressSection from './AddressSection';
import OrderSection from './OrderSection';

import { HiX } from 'react-icons/hi';

import { useNavigate } from 'react-router-dom';

export default function Profile() {
  const toast = useToast();
  const { user } = useAuth();
  const [roleColor, setRoleColor] = useState('gray');
  const { addresses, addAddress, deleteAddress } = useAddress();
  const { orders, fetchOrders, payForOrder } = useOrders();

  const navigate = useNavigate();

  useEffect(() => {
    console.log(user);
    if (user === null) return;
    setRoleColor(stringToColor(user.role));
  }, [user]);

  useEffect(() => {
    console.log(orders);
  }, [orders]);

  const getOrders = async (params) => {
    await fetchOrders(params);
  };

  const handlePayForOrder = async (orderId) => {
    const response = await payForOrder(orderId);
    if (response.success === true) {
      toast({
        title: 'Замовлення оплачено',
        status: 'success',
        duration: 5000,
        isClosable: true,
      });
    }
    if (response.success === false) {
      toast({
        title: 'Помилка оплати замовлення',
        description: response.error,
        status: 'error',
        duration: 5000,
        isClosable: true,
      });
    }
  };

  return (
    <Flex justify='center' align='start' flexGrow={1}>
      {user ? (
        <Box p={6} m={20} w='100%'>
          <Flex justify='space-between' align='center' mb={4}>
            <Flex align='center' mb={4}>
              <Avatar size='lg' mr={4} />
              <Box>
                <Text fontSize='xl' fontWeight='bold'>
                  {user.firstName} {user.lastName}
                </Text>
                <CustomBadge color={roleColor}>{user.role}</CustomBadge>
              </Box>
            </Flex>
            <Button
              onClick={() => {
                navigate('/logout');
              }}
              colorScheme='red'
              leftIcon={<HiX />}
            >
              Вийти
            </Button>
          </Flex>
          {user.role === 'user' && (
            <>
              <OrderSection
                orders={orders}
                fetchOrders={getOrders}
                payForOrder={handlePayForOrder}
              />
              <AddressSection
                addresses={addresses}
                addAddress={addAddress}
                deleteAddress={deleteAddress}
              />
            </>
          )}
        </Box>
      ) : (
        <Text>Loading...</Text>
      )}
    </Flex>
  );
}
