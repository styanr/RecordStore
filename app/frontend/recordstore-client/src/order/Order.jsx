import React, { useState } from 'react';
import {
  Box,
  Flex,
  Text,
  Heading,
  Step,
  StepDescription,
  StepIcon,
  StepIndicator,
  StepNumber,
  StepSeparator,
  StepStatus,
  StepTitle,
  Stepper,
  useSteps,
  Button,
  useToast,
} from '@chakra-ui/react';

import useAuth from '../hooks/useAuth';

import useCart from '../hooks/useCart';

import useAddress from '../hooks/useAddress';

import useOrders from '../hooks/useOrders';

import CartItems from './CartItems';
import ContactInfo from './ContactInfo';
import AddressForm from '../common/AddressForm';

const steps = [
  { title: 'Кошик', description: 'Перевірте ваше замовлення' },
  {
    title: 'Контактна інформація',
    description: 'Перевірте ваші контактні дані',
  },
  { title: 'Адреса доставки', description: 'Вкажіть адресу доставки' },
];

const Order = () => {
  const { activeStep, setActiveStep } = useSteps({
    index: 0,
    count: steps.length,
  });

  const toast = useToast();

  const { cart } = useCart();
  const { user } = useAuth();
  const { addresses } = useAddress();
  const { createOrder } = useOrders();

  const [newAddress, setNewAddress] = useState({
    city: '',
    street: '',
    building: '',
    apartment: '',
    region: '',
  });

  const handleAddressChange = (e) => {
    const { name, value } = e.target;
    setNewAddress({ ...newAddress, [name]: value });
  };

  const handleSubmit = async () => {
    const { success, error } = await createOrder(newAddress);

    if (success) {
      toast({
        title: 'Замовлення успішно оформлено',
        description: 'Для перегляду статусу замовлення перейдіть у ваш профіль',
        status: 'success',
        duration: 5000,
        isClosable: true,
      });
    } else {
      toast({
        title: 'Помилка при оформленні замовлення',
        description: error,
        status: 'error',
        duration: 5000,
        isClosable: true,
      });
    }
  };

  const onClose = () => {
    setNewAddress({
      city: '',
      street: '',
      building: '',
      apartment: '',
      region: '',
    });
  };

  return (
    <Box mt={5} mx={10} flexGrow={1}>
      <Heading as='h2' size='xl'>
        Оформлення замовлення
      </Heading>
      <Flex mt={12} w='70rem' alignItems='center' flexDir='column' mx='auto'>
        <Stepper index={activeStep} w='full'>
          {steps.map((step, index) => (
            <Step key={index} onClick={() => setActiveStep(index)}>
              <StepIndicator>
                <StepStatus
                  complete={<StepIcon />}
                  incomplete={<StepNumber />}
                  active={<StepNumber />}
                />
              </StepIndicator>

              <Box flexShrink='0'>
                <StepTitle>{step.title}</StepTitle>
                <StepDescription>{step.description}</StepDescription>
              </Box>

              <StepSeparator />
            </Step>
          ))}
        </Stepper>
        <Box w='full' mt='5' px='5'>
          {activeStep === 0 && (
            <>
              <Heading as='h3' size='lg'>
                Кошик
              </Heading>
              {cart.items === undefined || cart.items.length === 0 ? (
                <Box></Box>
              ) : (
                <CartItems cart={cart} />
              )}
            </>
          )}
          {activeStep === 1 && (
            <>
              <Heading as='h3' size='lg'>
                Контактна інформація
              </Heading>
              <ContactInfo user={user} />
            </>
          )}
          {activeStep === 2 && (
            <>
              <Heading as='h3' size='lg'>
                Адреса доставки
              </Heading>
              <Text mt={3}>
                Виберіть збережену адресу або вкажіть нову адресу доставки
              </Text>
              <Flex gap={2}>
                {addresses.map((address) => (
                  <Button
                    key={address.id}
                    colorScheme='blue'
                    variant='outline'
                    mt={3}
                    onClick={() => setNewAddress(address)}
                  >
                    {address.city}, {address.street}, {address.building}
                    {address.apartment ? `, кв. ${address.apartment}` : ''}
                  </Button>
                ))}
              </Flex>
              <Box mt={3}>
                <AddressForm
                  newAddress={newAddress}
                  handleAddressChange={handleAddressChange}
                  handleSubmit={handleSubmit}
                  onClose={onClose}
                />
              </Box>
            </>
          )}
        </Box>
      </Flex>
    </Box>
  );
};

export default Order;
