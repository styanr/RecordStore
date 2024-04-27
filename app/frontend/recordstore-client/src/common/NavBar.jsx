import {
  Box,
  Flex,
  HStack,
  IconButton,
  Menu,
  useDisclosure,
  useColorModeValue,
  Stack,
  Button,
  Drawer,
  DrawerBody,
  DrawerFooter,
  DrawerHeader,
  DrawerOverlay,
  DrawerContent,
  DrawerCloseButton,
  NumberInput,
  NumberInputField,
  NumberInputStepper,
  NumberIncrementStepper,
  NumberDecrementStepper,
  Heading,
} from '@chakra-ui/react';

import { FaCartShopping } from 'react-icons/fa6';
import { HiUser, HiUserMinus } from 'react-icons/hi2';
import { DeleteIcon } from '@chakra-ui/icons';

import { Link } from 'react-router-dom';

import useAuth from '../auth/useAuth';
import useCart from '../cart/useCart';

import { useState, useEffect, useRef } from 'react';
import { useNavigate } from 'react-router-dom';

const links = [
  {
    name: 'Головна сторінка',
    path: '/',
  },
];

const adminLinks = [
  {
    name: 'Головна сторінка',
    path: '/dashboard',
  },
];

const employeeLinks = [
  {
    name: 'Головна сторінка',
    path: '/employee-dashboard',
  },
  {
    name: 'Продукти',
    path: '/products',
  },
];

const guestLinks = [
  {
    name: 'Головна сторінка',
    path: '/',
  },
  {
    name: 'Увійти',
    path: '/login',
  },
];

const NavLink = (props) => {
  const { name, path } = props;

  return (
    <Box
      as={Link}
      to={path}
      px={2}
      py={1}
      rounded={'md'}
      _hover={{
        textDecoration: 'none',
        bg: useColorModeValue('gray.200', 'gray.700'),
      }}
    >
      {name}
    </Box>
  );
};

export default function Simple() {
  const { isOpen, onOpen, onClose } = useDisclosure();
  const btnRef = useRef();
  const { isAuthenticated, user } = useAuth();
  const [Links, setLinks] = useState(links);

  const navigate = useNavigate();

  const { cart, addToCart, updateCart, removeFromCart } = useCart();

  useEffect(() => {
    console.log(cart);
  }, [cart]);

  //   const Links = user && user.role === 'admin' ? adminLinks : links;

  useEffect(() => {
    if (user && user.role === 'admin') {
      setLinks(adminLinks);
    }
    if (user && user.role === 'employee') {
      setLinks(employeeLinks);
    } else if (!isAuthenticated) {
      setLinks(guestLinks);
    } else {
      setLinks(links);
    }
  }, [user]);

  return (
    <>
      <Drawer
        isOpen={isOpen}
        placement='right'
        onClose={onClose}
        finalFocusRef={btnRef}
      >
        <DrawerOverlay />
        <DrawerContent>
          <DrawerCloseButton />
          <DrawerHeader>Ваший кошик</DrawerHeader>

          <DrawerBody>
            {cart.items === undefined || cart.items.length === 0 ? (
              <Box>Кошик порожній</Box>
            ) : (
              <Box>
                {cart.items.map((item) => (
                  <Box key={item.product.id}>
                    <Heading size='sm' my='3'>
                      {item.product.title}
                    </Heading>
                    <HStack>
                      <NumberInput
                        defaultValue={item.quantity}
                        min={1}
                        onChange={(value) => updateCart(item.product.id, value)}
                      >
                        <NumberInputField />
                        <NumberInputStepper>
                          <NumberIncrementStepper />
                          <NumberDecrementStepper />
                        </NumberInputStepper>
                      </NumberInput>
                      <IconButton
                        aria-label='Видалити'
                        icon={<DeleteIcon />}
                        onClick={() => removeFromCart(item.product.id)}
                      />
                    </HStack>
                  </Box>
                ))}
                <Box>Загальна ціна: {cart.totalPrice} грн</Box>
              </Box>
            )}
          </DrawerBody>

          <DrawerFooter>
            <Button variant='outline' mr={3} onClick={onClose}>
              Закрити
            </Button>
            <Button
              colorScheme='blue'
              onClick={() => {
                navigate('/order');
                onClose();
              }}
            >
              Замовити
            </Button>
          </DrawerFooter>
        </DrawerContent>
      </Drawer>
      <Box bg={useColorModeValue('gray.100', 'gray.900')} px={4}>
        <Flex h={16} alignItems={'center'} justifyContent={'space-between'}>
          <HStack spacing={8} alignItems={'center'}>
            <Box>Record Store</Box>
            <HStack
              as={'nav'}
              spacing={4}
              display={{ base: 'none', md: 'flex' }}
            >
              {Links.map((link, index) => (
                <NavLink key={index} {...link} />
              ))}
            </HStack>
          </HStack>
          <Flex alignItems={'center'}>
            {isAuthenticated && (
              <Menu>
                <IconButton
                  aria-label='Кошик'
                  icon={<FaCartShopping />}
                  ref={btnRef}
                  onClick={onOpen}
                />
                <IconButton
                  aria-label='Профіль'
                  icon={<HiUser />}
                  onClick={() => navigate('/me')}
                />
              </Menu>
            )}
          </Flex>
        </Flex>
        <Box pb={4} display={{ md: 'none' }}>
          <Stack as={'nav'} spacing={4}>
            {Links.map((link, index) => (
              <NavLink key={index}>{link}</NavLink>
            ))}
          </Stack>
        </Box>
      </Box>
    </>
  );
}
