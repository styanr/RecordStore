import React from 'react';
import {
  Box,
  Flex,
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
} from '@chakra-ui/react';

import useAuth from '../auth/useAuth';

import useCart from '../cart/useCart';
import CartItems from './CartItems';
import ContactInfo from './ContactInfo';

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

  const { cart } = useCart();
  const { user } = useAuth();

  return (
    <Box mt={5} mx={10}>
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
        </Box>
      </Flex>
    </Box>
  );
};

export default Order;
